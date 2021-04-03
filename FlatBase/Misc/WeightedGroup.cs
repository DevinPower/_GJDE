using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FlatBase.Misc
{
    public class weightpool
    {
        public int maxSpread = 100;
        public int points { get; set; }
        Dictionary<WeightedGroup, int> sums = new Dictionary<WeightedGroup, int>();

        public weightpool()
        {
            points = 100;
        }

        public int registerValue(WeightedGroup wg, int val)
        {
            if (sums.ContainsKey(wg))
            {
                sums[wg] = val;
            }
            else
            {
                sums.Add(wg, val);
            }

            int tV = 0;
            int i = 0;
            foreach (KeyValuePair<WeightedGroup, int> entry in sums)
            {
                if (entry.Key != wg)
                    tV += entry.Value;
                i++;
            }

            return Math.Max(0, maxSpread - tV);
        }

        public void setValue(WeightedGroup wg, int val)
        {
            if (sums.ContainsKey(wg))
            {
                sums[wg] = val;
            }
            else
            {
                sums.Add(wg, val);
            }
        }
    }

    public class WeightedGroup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public weightpool tiedWP;
        int points = 0;

        int curMax = 100;

        public int POINTS
        {
            get 
            {
                return points;
            }
            set 
            {
                int v = value;
                curMax = tiedWP.registerValue(this, v);
                if (v > curMax)
                {
                    v = curMax;
                    tiedWP.setValue(this, v);
                }

                

                points = v;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("POINTS"));
            }
        }

        public WeightedGroup(weightpool w)
        {
            tiedWP = w;
        }
    }
}
