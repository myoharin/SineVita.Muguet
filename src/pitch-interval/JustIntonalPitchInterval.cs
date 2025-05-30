namespace SineVita.Muguet {
    public sealed class JustIntonalPitchInterval : PitchInterval { 
        // * Properties
        private (int Numerator, int Denominator) _ratio;
        public (int Numerator, int Denominator) Ratio { // strictly maintained as coprimes
            get => _ratio; 
            set {
                _ratio = value;
                var lcmNum = _Lcm(_ratio.Numerator, _ratio.Denominator);
                while (lcmNum != 1) {
                    _ratio =  (_ratio.Numerator / lcmNum, _ratio.Numerator / lcmNum);
                    lcmNum = _Lcm(_ratio.Numerator, _ratio.Denominator);
                }
            }
        }

        private const int DefaultRatioEstimateToleranceCent = 3;

        // * Statics
        private static int _Lcm(int a, int b) {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (a != b) {
                if (a < b) {b -= a;}
                if (a > b) {a -= b;}
            }
            return a;
        }

        private static IReadOnlyList<int> _defaultPrimeLimits = new List<int>() {
            2, 3, 5
        }.AsReadOnly();
        public static (int Numerator, int Denominator) EstimateRatio(double ratio, ICollection<int>? primeLimits = null) { // ! NOT DONE
            throw new NotImplementedException();
        }
        // TODO This function can have many different varation, need to provide a range as welll
        // TODO OK just get rid of all the functions below that allows raw frequencyRatio, or idk.
        // TODO there needs to be a parameterless converter either way

        // * Constructor
        public JustIntonalPitchInterval((int, int) justRatio, int centOffsets = 0) : base(centOffsets) => Ratio = justRatio;
        public JustIntonalPitchInterval(int numerator, int denominator, int centOffsets = 0) : base(centOffsets) => Ratio = (numerator, denominator);
        
        public JustIntonalPitchInterval(double frequencyRatio, int centOffsets = 0) : this(EstimateRatio(frequencyRatio), centOffsets) { }
        public JustIntonalPitchInterval(double frequencyRatio, ICollection<int>? primeLimits = null, int centOffsets = 0) : this(EstimateRatio(frequencyRatio, primeLimits), centOffsets) { }

        // * Overrides
        public override void Invert() {
            CentOffsets *= -1;
            Ratio = (Ratio.Denominator, Ratio.Numerator);
        }
        public override double GetFrequencyRatio()  {
            return Math.Pow(2, CentOffsets / 1200.0) * Ratio.Numerator / Ratio.Denominator;
        }
        public override string ToJson() {
            return string.Concat(
                "{",
                $"\"Ratio\": {Ratio},",
                $"\"Type\": \"{GetType()}\",",
                $"\"CentOffsets\": {CentOffsets}",
                "}"
            );
        }
        public override object Clone() {
            return new JustIntonalPitchInterval(Ratio, CentOffsets);
        }

        private void Increment(PitchInterval interval, double toleranceRatio) {
            if (interval is JustIntonalPitchInterval justInterval) {
                CentOffsets += justInterval.CentOffsets;
                Increment(justInterval.Ratio);
            } 
            else {
                Increment(interval.FrequencyRatio, toleranceRatio);
            }
        }
        private void Increment(PitchInterval interval, int tolerenceCents) {
            Increment(interval, Math.Pow(2,tolerenceCents/1200d));
        }
        public override void Increment(PitchInterval interval) {
            Increment(interval, DefaultRatioEstimateToleranceCent);
        }
        private void Increment(double ratio, double toleranceRatio = 1) { // ! NOT DONE
            throw new NotImplementedException();
        }
        private void Increment((int Numerator, int Denominator) ratio) {
            var newRatio = (ratio.Numerator * _ratio.Numerator, ratio.Denominator * _ratio.Denominator);
            _ratio = newRatio; // Automatically reduced
        }
        
        private void Decrement(PitchInterval interval, double toleranceRatio) {
            if (interval is JustIntonalPitchInterval justInterval) {
                CentOffsets += justInterval.CentOffsets;
                Decrement(justInterval.Ratio);
            } 
            else {
                Decrement(interval.FrequencyRatio, toleranceRatio);
            }
        }
        private void Decrement(PitchInterval interval, int tolerenceCents) {
            Decrement(interval, Math.Pow(2,tolerenceCents/1200d));
        }
        public override void Decrement(PitchInterval interval) {
            Decrement(interval, DefaultRatioEstimateToleranceCent);
        }
        private void Decrement(double ratio, double toleranceRatio = 1) { // ! NOT DONE
            throw new NotImplementedException();
        }
        private void Decrement((int Numerator, int Denominator) ratio) {
            var newRatio = (ratio.Denominator * _ratio.Numerator, ratio.Numerator * _ratio.Denominator);
            _ratio = newRatio; // Automatically reduced
        }

        public JustIntonalPitchInterval Incremented(double ratio) {
            var interval = (JustIntonalPitchInterval)Clone();
            interval.Increment(ratio, 0);
            return interval;
        }
        public JustIntonalPitchInterval Incremented((int Numerator, int Denominator) ratio) {
            var interval = (JustIntonalPitchInterval)Clone();
            interval.Increment(ratio);
            return interval;
        }
        public JustIntonalPitchInterval Decremented(double ratio) {
            var interval = (JustIntonalPitchInterval)Clone();
            interval.Decrement(ratio);
            return interval;
        }
        public JustIntonalPitchInterval Decremented((int Numerator, int Denominator) ratio) {
            var interval = (JustIntonalPitchInterval)Clone();
            interval.Decrement(ratio);
            return interval;
        }

        // * Operations
        public static JustIntonalPitchInterval operator +(JustIntonalPitchInterval interval, double ratio) {
            interval.Increment(ratio);
            return interval;
        }
        public static JustIntonalPitchInterval operator +(double ratio, JustIntonalPitchInterval interval) {
            interval.Increment(ratio);
            return interval;
        }
        public static JustIntonalPitchInterval operator -(JustIntonalPitchInterval interval, double ratio) {
            interval.Decrement(ratio);
            return interval;
        }

        public static JustIntonalPitchInterval operator +(JustIntonalPitchInterval interval, (int Numerator, int Denominator) ratio) {
            interval.Increment(ratio);
            return interval;
        }
        public static JustIntonalPitchInterval operator +((int Numerator, int Denominator) ratio, JustIntonalPitchInterval interval) {
            interval.Increment(ratio);
            return interval;
        }
        public static JustIntonalPitchInterval operator -(JustIntonalPitchInterval interval, (int Numerator, int Denominator) ratio) {
            interval.Decrement(ratio);
            return interval;
        }
    
    }

}