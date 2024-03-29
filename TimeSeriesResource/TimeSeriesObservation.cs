﻿using System;

namespace WIM.Resources.TimeSeries
{
    public class TimeSeriesObservation
    {
        #region Properties
        public DateTime Date { get; private set; }
        public Double? Value { get; private set; }
        public string DataCode { get; private set; }
        #endregion
        #region Constructor
        public TimeSeriesObservation(DateTime d, Double? v, string code)
        {
            this.Date = d;
            this.Value = v;
            this.DataCode = code;
        }
        public TimeSeriesObservation(DateTime d, Double? v)
        {
            this.Date = d;
            this.Value = v;
        }
        #endregion
    }
}
