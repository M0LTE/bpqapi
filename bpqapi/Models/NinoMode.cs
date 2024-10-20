namespace bpqapi.Models;

public record struct NinoMode
{
    public required int Id { get; init; }
    public required string Display { get; init; }
    public required bool Il2p { get; init; }
    public required bool Crc { get; init; }
    public required int Bitrate { get; init; }
    public required bool RecommendedForNewLinks { get; init; }
    public int? MinFirmware { get; set; }
    public bool? FmWide { get; init; }
    public string? Remark { get; init; }

    public static NinoMode Afsk300 = new() { Id = 12, Display = "AFSK300", Bitrate = 300, Il2p = false, Crc = false, Remark = "\"Classic\" HF packet", RecommendedForNewLinks = false };
    public static NinoMode Afsk1200 = new() { Id = 6, Display = "AFSK1200", Bitrate = 1200, Il2p = false, Crc = false, Remark = "\"Classic\" VHF FM packet / APRS", RecommendedForNewLinks = false };
    public static NinoMode Gfsk9600 = new() { Id = 0, Display = "GFSK9600", Bitrate = 9600, Il2p = false, Crc = false, FmWide = true, Remark = "\"Classic\" UHF FM packet (G3RUH)", RecommendedForNewLinks = false };

    public static NinoMode C4Fsk19200Il2pc = new() { Id = 1, Display = "C4FSK19200 IL2Pc", Bitrate = 19200, Il2p = true, Crc = true, FmWide = true, Remark = "Highest current speeds", RecommendedForNewLinks = true, MinFirmware = 41 };
    public static NinoMode C4Fsk9600Il2pc = new() { Id = 3, Display = "C4FSK9600 IL2Pc", Bitrate = 9600, Il2p = true, Crc = true, FmWide = false, Remark = "9k6 on 2m", RecommendedForNewLinks = true, MinFirmware = 41 };

    public static NinoMode Gfsk4800Il2pc = new() { Id = 4, Display = "GFSK4800 IL2Pc", Bitrate = 4800, Il2p = true, Crc = true, FmWide = false, Remark = "Narrow mode which requires data pins", RecommendedForNewLinks = false, MinFirmware = 31 };
    public static NinoMode Qpsk3600Il2pc = new() { Id = 5, Display = "QPSK3600 IL2Pc", Bitrate = 3600, Il2p = true, Crc = true, FmWide = false, Remark = "Narrow mode which works through spk/mic", RecommendedForNewLinks = true, MinFirmware = 39 };

    public static NinoMode Afsk300Il2pc = new() { Id = 14, Display = "AFSK300 IL2Pc", Bitrate = 300, Il2p = true, Crc = true, Remark = "Preferred slow FSK mode for HF - 40m slot 1", RecommendedForNewLinks = true, MinFirmware = 31 };
    public static NinoMode Bpsk300Il2pc = new() { Id = 8, Display = "BPSK300 IL2Pc", Bitrate = 300, Il2p = true, Crc = true, Remark = "Preferred slow PSK mode for HF - 40m slot 3", RecommendedForNewLinks = true, MinFirmware = 31 };

    public static NinoMode Qpsk600Il2pc = new() { Id = 9, Display = "QPSK600 IL2Pc", Bitrate = 600, Il2p = true, Crc = true, Remark = "Same bw as BPSK300 but needs more SNR", RecommendedForNewLinks = true, MinFirmware = 31 };
    public static NinoMode Bpsk1200Il2pc = new() { Id = 10, Display = "BPSK1200 IL2Pc", Bitrate = 1200, Il2p = true, Crc = true, Remark = "2.4kHz wide, fast for HF, needs plenty of SNR", RecommendedForNewLinks = true, MinFirmware = 31 };
    public static NinoMode Qpsk2400Il2pc = new() { Id = 11, Display = "QPSK2400 IL2Pc", Bitrate = 2400, Il2p = true, Crc = true, Remark = "2.4kHz wide, faster but more SNR needed", RecommendedForNewLinks = true, MinFirmware = 31 };

    public static NinoMode Gfsk9600Il2pc = new() { Id = 2, Display = "GFSK9600 IL2Pc", Bitrate = 9600, Il2p = true, Crc = true, FmWide = true, Remark = "IL2Pc version of classic G3RUH modem", RecommendedForNewLinks = false, MinFirmware = 31 };
    public static NinoMode Afsk300Il2p = new() { Id = 13, Display = "AFSK300 IL2P", Bitrate = 300, Il2p = true, Crc = false, Remark = "Prefer AFSK300 IL2Pc to this", RecommendedForNewLinks = false };
    public static NinoMode Afsk1200Il2p = new() { Id = 7, Display = "AFSK1200 IL2P", Bitrate = 1200, Il2p = true, Crc = false, Remark = "Prefer AFSK1200Il2pc to this", RecommendedForNewLinks = false };

    public static NinoMode[] All = [Afsk300, Afsk1200, Gfsk9600, C4Fsk19200Il2pc, C4Fsk9600Il2pc, Gfsk4800Il2pc, Qpsk3600Il2pc, Afsk300Il2pc, Bpsk300Il2pc, Qpsk600Il2pc, Bpsk1200Il2pc, Qpsk2400Il2pc, Gfsk9600Il2pc, Afsk300Il2p, Afsk1200Il2p];
}
