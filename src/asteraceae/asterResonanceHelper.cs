using SineVita.Muguet.Asteraceae.Cosmosia;
namespace SineVita.Muguet.Asteraceae {
    public class ResonanceHelper{

        public static string? ParametersFolderPath = null; //?? Path.Combine("assets", "resonator-parameters");
        public static bool FolderPathSet { get {return ParametersFolderPath == null;} }
        protected const int DefaultTimeOutDeletionDuration = 32768; // ms

        // ! ALL THIS NEEDS TO BE FIXED TO GENERIC PARAMETERS

        // * Manual Parameter Cache Management
        protected static Dictionary<int, ResonatorParameter> ResonatorParamaters { get; set; } = new Dictionary<int, ResonatorParameter>();
        public static void ResonatorParamatersAddCache(int newResonatorParamaterID, bool autoDeletionTimer = false) {
            ResonatorParameterCosmosia newResonatorParameter = new ResonatorParameterCosmosia(newResonatorParamaterID);
            ResonatorParamaters.Add(newResonatorParamaterID, newResonatorParameter);
            if (autoDeletionTimer) {StartAutoDeletionTimer(newResonatorParamaterID);}
        }
        public static void ResonatorParamatersAddCache(string newResonatorParamaterPath, bool autoDeletionTimer = false) {
            ResonatorParameterCosmosia newResonatorParameter;
            try{
                newResonatorParameter = new ResonatorParameterCosmosia(newResonatorParamaterPath);
            }
            catch(Exception) {
                throw new FileNotFoundException("FileNotFound");
            }
            if (int.TryParse(newResonatorParamaterPath.Split("\\").Last().Split(".")[0], out int result))
            {
                int ID = result;
                ResonatorParamaters.Add(ID, newResonatorParameter);
                if (autoDeletionTimer) {StartAutoDeletionTimer(ID);}
            }
            else{
                throw new FileNotFoundException("IDnotFound");
            }
        }
                
        public static bool ResonatorParamatersDeleteCache(int deletionResonatorParamaterID) {
            return ResonatorParamaters.Remove(deletionResonatorParamaterID);
        }
        public static bool ResonatorParamatersDeleteCache(string deletionResonatorParamaterPath) {
            if (int.TryParse(deletionResonatorParamaterPath.Split("\\").Last().Split(".")[0], out int result))
            {
                int ID = result;
                return ResonatorParamaters.Remove(ID);
            }
            else{
                throw new FileNotFoundException("ParamaterIDNotSpecified");
            }
        }

        // * Auto Parameter Deletion
        public static void IncrementTimerInGameTime(int currentRunTime, double deltaTime, int TimeOutDeletionDuration = DefaultTimeOutDeletionDuration) {
            foreach (KeyValuePair<int, ResonatorParameter> keyPair in ResonatorParamaters)
            {
                if (currentRunTime - keyPair.Value.RunTimeLastFetched > TimeOutDeletionDuration){
                    ResonatorParamatersDeleteCache(keyPair.Key);
                } else {
                    keyPair.Value.RunTimeLastFetched += (int)(deltaTime * 1000);
                }
            }
        }
        public static async void StartAutoDeletionTimer(int resonatorParameterID, int timeOutDuration = DefaultTimeOutDeletionDuration) {
            await Task.Delay(timeOutDuration);
            ResonatorParamatersDeleteCache(resonatorParameterID);
        }

        // * Safe Parameter Access
        public static ResonatorParameter GetResonatorParameter(int ResonatorParamaterID) {
            try {
                return ResonatorParamaters[ResonatorParamaterID];
            }
            catch (Exception) { // does not exist
                ResonatorParamatersAddCache(ResonatorParamaterID);
                return ResonatorParamaters[ResonatorParamaterID];
            }
        }
             
    }
}