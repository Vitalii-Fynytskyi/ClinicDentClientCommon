using System.Collections.Generic;

namespace ClinicDentClientCommon.Model
{
    public enum AssetType : byte
    {
        Bond = 0,
        Dentin = 1,
        Enamel = 2,
        CanalMethod = 3,
        Sealer = 4,
        Cement = 5,
        Technician = 6,
        Pin = 7,
        Operation = 8,
        Calcium = 9
    }
    /// <summary>
    /// Assets represents additional params of stages such as used materials in work
    /// </summary>
    public class StageAsset
    {
        public int Id { get; set; }
        public AssetType Type { get; set; }
        public string Value { get; set; }
        public static List<StageAsset> Operations { get; set; }
        public static List<StageAsset> Bonds { get; set; }
        public static List<StageAsset> CanalMethods { get; set; }
        public static List<StageAsset> Cements { get; set; }
        public static List<StageAsset> Calciums { get; set; }
        public static List<StageAsset> Dentins { get; set; }
        public static List<StageAsset> Enamels { get; set; }
        public static List<StageAsset> Pins { get; set; }
        public static List<StageAsset> Sealers { get; set; }
        public static List<StageAsset> Technicians { get; set; }
    }
}
