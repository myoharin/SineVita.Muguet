using System.Text.Json;

namespace SineVita.Muguet
{
    public abstract partial class PitchInterval :
            IReadOnlyPitchInterval, 
            ICloneable
    {
        // * Properties
        private int _centOffsets;
        public virtual int CentOffsets {
            get => _centOffsets;
            set {
                _centOffsets = 0;
                _centOffsets = value;
            }
        }

        // * Derived Gets
        public virtual bool IsUnison => Math.Abs(FrequencyRatio - 1.0d) < 0.001;
        public bool IsAbsolute => IsPositive(this) || IsUnison;

        public virtual double FrequencyRatio => GetFrequencyRatio();
        public int ToMidiIndex => (int)MidiPitchInterval.ToIndex(this);
        public float ToMidiValue => MidiPitchInterval.ToIndex(this, false);
        
        // ? Harmony Helper Derives
        public virtual string IntervalName => HarmonyHelper.HtzToIntervalName(FrequencyRatio);

        // * Statics
        public static PitchInterval Empty => new FloatPitchInterval(1.0);

        public static string[] IntervalNames { get; } = new[] {
            // Populate with actual interval names
            "R", "m2", "M2", "m3", "M3", "P4", "T1", "P5", "m6", "M6", "m7", "M7",
            "O1", "m9", "M9", "m10", "M10", "P11", "T2", "P12", "m13", "M13", "m14", "M14",
            "O2", "m16", "M16", "m17", "M17", "P18", "T3", "P19", "m20", "M20", "m21", "M21",
            "O3", "m23", "M23", "m24", "M24", "P25", "T4", "P26", "m27", "M27", "m28", "M28",
            "O4"
        };

        public static PitchInterval Octave => new JustIntonalPitchInterval((2, 1));
        public static PitchInterval Perfect5th => new JustIntonalPitchInterval((3, 2));
        public static PitchInterval Perfect4th => new JustIntonalPitchInterval((4, 3));
        public static PitchInterval PerfectFifth => new JustIntonalPitchInterval((3, 2));
        public static PitchInterval PerfectFourth => new JustIntonalPitchInterval((4, 3));
        public static PitchInterval Unison => Empty;
        public static PitchInterval Default => Empty;

        // * FromJson
        public static PitchInterval FromJson(string jsonString) {var jsonDocument = JsonDocument.Parse(jsonString);
            var rootElement = jsonDocument.RootElement;
            var type = System.Type.GetType(rootElement.GetProperty("Type").GetString() ?? throw new ArgumentException("Invalid JsonString"));
            switch (type)
            {
                case Type t when t == typeof(FloatPitchInterval):
                    return new FloatPitchInterval(rootElement.GetProperty("FrequencyRatio").GetDouble());
                case Type t when t == typeof(JustIntonalPitchInterval):
                    var justFrequency = rootElement.GetProperty("Ratio");
                    return new JustIntonalPitchInterval(
                        (justFrequency.GetProperty("Numerator").GetInt32(), justFrequency.GetProperty("Denominator").GetInt32()),
                        rootElement.GetProperty("CentOffsets").GetInt32()
                    );
                case Type t when t == typeof(CustomTetPitchInterval):
                    return new CustomTetPitchInterval(
                        rootElement.GetProperty("Base").GetInt32(),
                        rootElement.GetProperty("PitchIntervalIndex").GetInt32(),
                        rootElement.GetProperty("CentOffsets").GetInt32()
                    );
                case Type t when t == typeof(MidiPitchInterval):
                    return new MidiPitchInterval(
                        rootElement.GetProperty("PitchIntervalIndex").GetInt32(),
                        rootElement.GetProperty("CentOffsets").GetInt32()
                    );
                case Type t when t == typeof(CompoundPitchInterval):
                    var centOffsets = rootElement.GetProperty("CentOffsets").GetInt32();
                    var intervals = rootElement.GetProperty("Intervals")
                        .EnumerateArray()
                        .Select(intervalElement => PitchInterval.FromJson(intervalElement.GetRawText()))
                        .ToList();
                    return new CompoundPitchInterval(intervals, centOffsets);
                default:
                    throw new ArgumentException("Invalid pitch type");
            }
        }

        // * Constructor
        protected PitchInterval(int centOffsets = 0) { CentOffsets = centOffsets; }

        // * abstract overrides
        public PitchInterval ToPitchInterval() => (PitchInterval)Clone();

        public abstract double GetFrequencyRatio();
        public abstract string ToJson();

        public abstract void Invert();
        public abstract void Increment(PitchInterval interval);
        public abstract void Decrement(PitchInterval interval);

        // * Abstract Derived

        public PitchInterval Inverted() {
            var returnInterval = this; 
            returnInterval.Invert();
            return returnInterval;
        }
        public PitchInterval Incremented(PitchInterval interval) {
            var newInterval = (PitchInterval)this.Clone();
            newInterval.Increment(interval);
            return newInterval;
        }
        public PitchInterval Decremented(PitchInterval interval) {var newInterval = (PitchInterval)this.Clone();
            newInterval.Decrement(interval);
            return newInterval;
        }

    }
}