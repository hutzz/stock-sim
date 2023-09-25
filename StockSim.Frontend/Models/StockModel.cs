using System.Collections.Generic;

namespace StockSim.Frontend.Models {
    public class StockModel {
        public List<DateTime>? DateTimes { get; set; }
        public List<float>? Open { get; set; }
        public List<float>? Close { get; set; }
        public List<float>? High { get; set; }
        public List<float>? Low { get; set; }
        public List<float>? Volume { get; set; }
        public List<float>? Splits { get; set; }
        public List<float>? Dividends { get; set; }
    }
}
