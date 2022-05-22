using System.Net;

namespace balancer;

public static class WeightFixer
{
    public static void FixWeight(IGrouping<string, Entry> grouping)
    {
        //none of the math matters if minecraft not in the list. Skip 
        if (!grouping.Any(entry => string.Equals(entry.Source, "minecraft")))
        {
            return;
        }
        
        var calcs = new Dictionary<string, Calc>();

        //calculate weights
        foreach (var entry in grouping)
        {
            var source = entry.Source;
            if (!calcs.ContainsKey(source))
            {
                calcs.Add(source, new Calc());
            }

            calcs[source].Weight += entry.Weight;
        }
        
        //calculate pre percents
        var minecraftWeight = calcs["minecraft"].Weight;
        var totalWeight = calcs.Values.Sum(v => v.Weight);
        foreach (var calc in calcs)
        {
            if (string.Equals(calc.Key, "minecraft"))
            {
                calc.Value.PrePercent = new decimal(1);
            }
            else
            {
                calc.Value.PrePercent = calc.Value.Weight / (decimal)(minecraftWeight + calc.Value.Weight);
            }
            calc.Value.PostPercent = calc.Value.Weight / (decimal)(totalWeight);
            
            //fix low % / minecraft & lock
            if (calc.Value.PrePercent.CompareTo(new decimal(.05)) == -1)
            {
                calc.Value.DesiredPercent = calc.Value.PrePercent;
            }
        }
        
        //if there is no pre post diff, then don't bother with anything else
        if (calcs["minecraft"].PrePostDiff.CompareTo(0) == 0)
        {
            return;
        }

        var desiredDifferences = calcs.Values.Sum(v => v.PostDesiredDiff);
        var totalUnlockedPost= calcs.Values.Sum(v => v.DesiredPercent.CompareTo(0) == 0 ? v.PostPercent: 0);
        
        // special calculation for minecraft to never be below 50%
        var minecraftAdjust = (calcs["minecraft"].PostPercent / totalUnlockedPost) * desiredDifferences ;
        var preFlooredMinecraftDesiredPercent = calcs["minecraft"].PostPercent - minecraftAdjust;

        calcs["minecraft"].DesiredPercent = Math.Max(new decimal(.5), preFlooredMinecraftDesiredPercent);
        
        desiredDifferences = calcs.Values.Sum(v => v.PostDesiredDiff);
        totalUnlockedPost= calcs.Values.Sum(v => v.DesiredPercent.CompareTo(0) == 0 ? v.PostPercent: 0);

        foreach (var calc in calcs)
        {
            if (calc.Value.DesiredPercent.CompareTo(0) != 0)
            {
                continue;
            }
            
            //if there is no pre post diff, then don't bother with anything else
            if (calc.Value.PrePostDiff.CompareTo(0) == 0)
            {
                return;
            }
            
            var adjust = (calc.Value.PostPercent / totalUnlockedPost) * desiredDifferences;
            calc.Value.DesiredPercent = calc.Value.PostPercent - adjust;
        }


        foreach (var entry in grouping)
        {
            var calc = calcs[entry.Source];
            var preVal = entry.Weight;
            var postVal = (int)Math.Round(entry.Weight * calc.AlterationMultiplier, MidpointRounding.AwayFromZero);
            if (postVal != preVal)
            {
                Console.WriteLine($"changing {entry.Biome} {entry.Entity} from {preVal} to {postVal}");
                entry.Weight = postVal;
            }

        }
    }

    private class Calc
    {
        public int Weight { get; set; }
        public decimal PrePercent { get; set; }
        public decimal PostPercent { get; set; }
        public decimal PrePostDiff => PrePercent - PostPercent;
        public decimal DesiredPercent { get; set; }
        public decimal PostDesiredDiff => DesiredPercent.CompareTo(0) != 0 ? DesiredPercent - PostPercent : 0;
        public decimal AlterationMultiplier => (DesiredPercent / PostPercent);
    }
}