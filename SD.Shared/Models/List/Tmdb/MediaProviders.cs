namespace SD.Shared.Models.List.Tmdb;

public class ProviderBase
{
    public int display_priority { get; set; }
    public string? logo_path { get; set; }
    public int provider_id { get; set; }
    public string? provider_name { get; set; }
    public string? provider_desciption { get; set; }
    public string? provider_link { get; set; }
}

public class Free : ProviderBase;

public class Ads : ProviderBase;

public class Flatrate : ProviderBase;

//public class FlatrateAndBuy : ProviderBase;

public class Rent : ProviderBase;

public class Buy : ProviderBase;

public class CountryBase
{
    public string? link { get; set; }
    public List<Free> free { get; set; } = [];
    public List<Ads> ads { get; set; } = [];
    public List<Flatrate> flatrate { get; set; } = [];
    //public List<FlatrateAndBuy> flatrate_and_buy { get; set; } = [];
    public List<Rent> rent { get; set; } = [];
    public List<Buy> buy { get; set; } = [];
}

public class AD : CountryBase;

public class AE : CountryBase;

public class AG : CountryBase;

public class AL : CountryBase;

public class AR : CountryBase;

public class AT : CountryBase;

public class AU : CountryBase;

public class BA : CountryBase;

public class BB : CountryBase;

public class BE : CountryBase;

public class BG : CountryBase;

public class BH : CountryBase;

public class BM : CountryBase;

public class BO : CountryBase;

public class BR : CountryBase;

public class BS : CountryBase;

public class CA : CountryBase;

public class CH : CountryBase;

public class CI : CountryBase;

public class CL : CountryBase;

public class CO : CountryBase;

public class CR : CountryBase;

public class CU : CountryBase;

public class CV : CountryBase;

public class CZ : CountryBase;

public class DE : CountryBase;

public class DK : CountryBase;

public class DO : CountryBase;

public class DZ : CountryBase;

public class EC : CountryBase;

public class EE : CountryBase;

public class EG : CountryBase;

public class ES : CountryBase;

public class FI : CountryBase;

public class FJ : CountryBase;

public class FR : CountryBase;

public class GB : CountryBase;

public class GF : CountryBase;

public class GH : CountryBase;

public class GI : CountryBase;

public class GP : CountryBase;

public class GQ : CountryBase;

public class GR : CountryBase;

public class GT : CountryBase;

public class HK : CountryBase;

public class HN : CountryBase;

public class HR : CountryBase;

public class HU : CountryBase;

public class ID : CountryBase;

public class IE : CountryBase;

public class IL : CountryBase;

public class IN : CountryBase;

public class IQ : CountryBase;

public class IS : CountryBase;

public class IT : CountryBase;

public class JM : CountryBase;

public class JO : CountryBase;

public class JP : CountryBase;

public class KE : CountryBase;

public class KR : CountryBase;

public class KW : CountryBase;

public class LB : CountryBase;

public class LC : CountryBase;

public class LI : CountryBase;

public class LT : CountryBase;

public class LV : CountryBase;

public class LY : CountryBase;

public class MA : CountryBase;

public class MC : CountryBase;

public class MD : CountryBase;

public class MK : CountryBase;

public class MT : CountryBase;

public class MU : CountryBase;

public class MX : CountryBase;

public class MY : CountryBase;

public class MZ : CountryBase;

public class NE : CountryBase;

public class NG : CountryBase;

public class NL : CountryBase;

public class NO : CountryBase;

public class NZ : CountryBase;

public class OM : CountryBase;

public class PA : CountryBase;

public class PE : CountryBase;

public class PF : CountryBase;

public class PH : CountryBase;

public class PK : CountryBase;

public class PL : CountryBase;

public class PS : CountryBase;

public class PT : CountryBase;

public class PY : CountryBase;

public class QA : CountryBase;

public class RO : CountryBase;

public class RS : CountryBase;

public class RU : CountryBase;

public class SA : CountryBase;

public class SC : CountryBase;

public class SE : CountryBase;

public class SG : CountryBase;

public class SI : CountryBase;

public class SK : CountryBase;

public class SM : CountryBase;

public class SN : CountryBase;

public class SV : CountryBase;

public class TC : CountryBase;

public class TH : CountryBase;

public class TN : CountryBase;

public class TR : CountryBase;

public class TT : CountryBase;

public class TW : CountryBase;

public class TZ : CountryBase;

public class UG : CountryBase;

public class US : CountryBase;

public class UY : CountryBase;

public class VA : CountryBase;

public class VE : CountryBase;

public class XK : CountryBase;

public class YE : CountryBase;

public class ZA : CountryBase;

public class ZM : CountryBase;

public class Results
{
    public AD? AD { get; set; }
    public AE? AE { get; set; }
    public AG? AG { get; set; }
    public AL? AL { get; set; }
    public AR? AR { get; set; }
    public AT? AT { get; set; }
    public AU? AU { get; set; }
    public BA? BA { get; set; }
    public BB? BB { get; set; }
    public BE? BE { get; set; }
    public BG? BG { get; set; }
    public BH? BH { get; set; }
    public BM? BM { get; set; }
    public BO? BO { get; set; }
    public BR? BR { get; set; }
    public BS? BS { get; set; }
    public CA? CA { get; set; }
    public CH? CH { get; set; }
    public CI? CI { get; set; }
    public CL? CL { get; set; }
    public CO? CO { get; set; }
    public CR? CR { get; set; }
    public CU? CU { get; set; }
    public CV? CV { get; set; }
    public CZ? CZ { get; set; }
    public DE? DE { get; set; }
    public DK? DK { get; set; }
    public DO? DO { get; set; }
    public DZ? DZ { get; set; }
    public EC? EC { get; set; }
    public EE? EE { get; set; }
    public EG? EG { get; set; }
    public ES? ES { get; set; }
    public FI? FI { get; set; }
    public FJ? FJ { get; set; }
    public FR? FR { get; set; }
    public GB? GB { get; set; }
    public GF? GF { get; set; }
    public GH? GH { get; set; }
    public GI? GI { get; set; }
    public GP? GP { get; set; }
    public GQ? GQ { get; set; }
    public GR? GR { get; set; }
    public GT? GT { get; set; }
    public HK? HK { get; set; }
    public HN? HN { get; set; }
    public HR? HR { get; set; }
    public HU? HU { get; set; }
    public ID? ID { get; set; }
    public IE? IE { get; set; }
    public IL? IL { get; set; }
    public IN? IN { get; set; }
    public IQ? IQ { get; set; }
    public IS? IS { get; set; }
    public IT? IT { get; set; }
    public JM? JM { get; set; }
    public JO? JO { get; set; }
    public JP? JP { get; set; }
    public KE? KE { get; set; }
    public KR? KR { get; set; }
    public KW? KW { get; set; }
    public LB? LB { get; set; }
    public LC? LC { get; set; }
    public LI? LI { get; set; }
    public LT? LT { get; set; }
    public LV? LV { get; set; }
    public LY? LY { get; set; }
    public MA? MA { get; set; }
    public MC? MC { get; set; }
    public MD? MD { get; set; }
    public MK? MK { get; set; }
    public MT? MT { get; set; }
    public MU? MU { get; set; }
    public MX? MX { get; set; }
    public MY? MY { get; set; }
    public MZ? MZ { get; set; }
    public NE? NE { get; set; }
    public NG? NG { get; set; }
    public NL? NL { get; set; }
    public NO? NO { get; set; }
    public NZ? NZ { get; set; }
    public OM? OM { get; set; }
    public PA? PA { get; set; }
    public PE? PE { get; set; }
    public PF? PF { get; set; }
    public PH? PH { get; set; }
    public PK? PK { get; set; }
    public PL? PL { get; set; }
    public PS? PS { get; set; }
    public PT? PT { get; set; }
    public PY? PY { get; set; }
    public QA? QA { get; set; }
    public RO? RO { get; set; }
    public RS? RS { get; set; }
    public RU? RU { get; set; }
    public SA? SA { get; set; }
    public SC? SC { get; set; }
    public SE? SE { get; set; }
    public SG? SG { get; set; }
    public SI? SI { get; set; }
    public SK? SK { get; set; }
    public SM? SM { get; set; }
    public SN? SN { get; set; }
    public SV? SV { get; set; }
    public TC? TC { get; set; }
    public TH? TH { get; set; }
    public TN? TN { get; set; }
    public TR? TR { get; set; }
    public TT? TT { get; set; }
    public TW? TW { get; set; }
    public TZ? TZ { get; set; }
    public UG? UG { get; set; }
    public US? US { get; set; }
    public UY? UY { get; set; }
    public VA? VA { get; set; }
    public VE? VE { get; set; }
    public XK? XK { get; set; }
    public YE? YE { get; set; }
    public ZA? ZA { get; set; }
    public ZM? ZM { get; set; }
}

public class MediaProviders
{
    public int id { get; set; }
    public Results? results { get; set; }

    public string? GetLink(Country region)
    {
        if (results == null) return "";

        return region switch
        {
            Country.AD => results.AD?.link,
            Country.AE => results.AE?.link,
            Country.AG => results.AG?.link,
            Country.AL => results.AL?.link,
            Country.AR => results.AR?.link,
            Country.AT => results.AT?.link,
            Country.AU => results.AU?.link,
            Country.BA => results.BA?.link,
            Country.BB => results.BB?.link,
            Country.BE => results.BE?.link,
            Country.BG => results.BG?.link,
            Country.BH => results.BH?.link,
            Country.BM => results.BM?.link,
            Country.BO => results.BO?.link,
            Country.BR => results.BR?.link,
            Country.BS => results.BS?.link,
            Country.CA => results.CA?.link,
            Country.CH => results.CH?.link,
            Country.CI => results.CI?.link,
            Country.CL => results.CL?.link,
            Country.CO => results.CO?.link,
            Country.CR => results.CR?.link,
            Country.CU => results.CU?.link,
            Country.CV => results.CV?.link,
            Country.CZ => results.CZ?.link,
            Country.DE => results.DE?.link,
            Country.DK => results.DK?.link,
            Country.DO => results.DO?.link,
            Country.DZ => results.DZ?.link,
            Country.EC => results.EC?.link,
            Country.EE => results.EE?.link,
            Country.EG => results.EG?.link,
            Country.ES => results.ES?.link,
            Country.FI => results.FI?.link,
            Country.FJ => results.FJ?.link,
            Country.FR => results.FR?.link,
            Country.GB => results.GB?.link,
            Country.GF => results.GF?.link,
            Country.GH => results.GH?.link,
            Country.GI => results.GI?.link,
            Country.GP => results.GP?.link,
            Country.GQ => results.GQ?.link,
            Country.GR => results.GR?.link,
            Country.GT => results.GT?.link,
            Country.HK => results.HK?.link,
            Country.HN => results.HN?.link,
            Country.HR => results.HR?.link,
            Country.HU => results.HU?.link,
            Country.ID => results.ID?.link,
            Country.IE => results.IE?.link,
            Country.IL => results.IL?.link,
            Country.IN => results.IN?.link,
            Country.IQ => results.IQ?.link,
            Country.IS => results.IS?.link,
            Country.IT => results.IT?.link,
            Country.JM => results.JM?.link,
            Country.JO => results.JO?.link,
            Country.JP => results.JP?.link,
            Country.KE => results.KE?.link,
            Country.KR => results.KR?.link,
            Country.KW => results.KW?.link,
            Country.LB => results.LB?.link,
            Country.LC => results.LC?.link,
            Country.LI => results.LI?.link,
            Country.LT => results.LT?.link,
            Country.LV => results.LV?.link,
            Country.LY => results.LY?.link,
            Country.MA => results.MA?.link,
            Country.MC => results.MC?.link,
            Country.MD => results.MD?.link,
            Country.MK => results.MK?.link,
            Country.MT => results.MT?.link,
            Country.MU => results.MU?.link,
            Country.MX => results.MX?.link,
            Country.MY => results.MY?.link,
            Country.MZ => results.MZ?.link,
            Country.NE => results.NE?.link,
            Country.NG => results.NG?.link,
            Country.NL => results.NL?.link,
            Country.NO => results.NO?.link,
            Country.NZ => results.NZ?.link,
            Country.OM => results.OM?.link,
            Country.PA => results.PA?.link,
            Country.PE => results.PE?.link,
            Country.PF => results.PF?.link,
            Country.PH => results.PH?.link,
            Country.PK => results.PK?.link,
            Country.PL => results.PL?.link,
            Country.PS => results.PS?.link,
            Country.PT => results.PT?.link,
            Country.PY => results.PY?.link,
            Country.QA => results.QA?.link,
            Country.RO => results.RO?.link,
            Country.RS => results.RS?.link,
            Country.RU => results.RU?.link,
            Country.SA => results.SA?.link,
            Country.SC => results.SC?.link,
            Country.SE => results.SE?.link,
            Country.SG => results.SG?.link,
            Country.SI => results.SI?.link,
            Country.SK => results.SK?.link,
            Country.SM => results.SM?.link,
            Country.SN => results.SN?.link,
            Country.SV => results.SV?.link,
            Country.TC => results.TC?.link,
            Country.TH => results.TH?.link,
            Country.TN => results.TN?.link,
            Country.TR => results.TR?.link,
            Country.TT => results.TT?.link,
            Country.TW => results.TW?.link,
            Country.TZ => results.TZ?.link,
            Country.UG => results.UG?.link,
            Country.US => results.US?.link,
            Country.UY => results.UY?.link,
            Country.VA => results.VA?.link,
            Country.VE => results.VE?.link,
            Country.XK => results.XK?.link,
            Country.YE => results.YE?.link,
            Country.ZA => results.ZA?.link,
            Country.ZM => results.ZM?.link,
            _ => null
        };
    }

    public List<ProviderBase> GetFreeListProviders(Country? region)
    {
        if (results == null) return [];

        return region switch
        {
            Country.AD => results.AD?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.AE => results.AE?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.AG => results.AG?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.AL => results.AL?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.AR => results.AR?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.AT => results.AT?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.AU => results.AU?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.BA => results.BA?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.BB => results.BB?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.BE => results.BE?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.BG => results.BG?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.BH => results.BH?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.BM => results.BM?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.BO => results.BO?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.BR => results.BR?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.BS => results.BS?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.CA => results.CA?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.CH => results.CH?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.CI => results.CI?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.CL => results.CL?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.CO => results.CO?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.CR => results.CR?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.CU => results.CU?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.CV => results.CV?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.CZ => results.CZ?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.DE => results.DE?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.DK => results.DK?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.DO => results.DO?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.DZ => results.DZ?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.EC => results.EC?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.EE => results.EE?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.EG => results.EG?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.ES => results.ES?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.FI => results.FI?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.FJ => results.FJ?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.FR => results.FR?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.GB => results.GB?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.GF => results.GF?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.GH => results.GH?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.GI => results.GI?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.GP => results.GP?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.GQ => results.GQ?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.GR => results.GR?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.GT => results.GT?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.HK => results.HK?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.HN => results.HN?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.HR => results.HR?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.HU => results.HU?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.ID => results.ID?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.IE => results.IE?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.IL => results.IL?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.IN => results.IN?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.IQ => results.IQ?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.IS => results.IS?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.IT => results.IT?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.JM => results.JM?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.JO => results.JO?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.JP => results.JP?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.KE => results.KE?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.KR => results.KR?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.KW => results.KW?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.LB => results.LB?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.LC => results.LC?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.LI => results.LI?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.LT => results.LT?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.LV => results.LV?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.LY => results.LY?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.MA => results.MA?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.MC => results.MC?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.MD => results.MD?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.MK => results.MK?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.MT => results.MT?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.MU => results.MU?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.MX => results.MX?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.MY => results.MY?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.MZ => results.MZ?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.NE => results.NE?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.NG => results.NG?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.NL => results.NL?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.NO => results.NO?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.NZ => results.NZ?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.OM => results.OM?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.PA => results.PA?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.PE => results.PE?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.PF => results.PF?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.PH => results.PH?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.PK => results.PK?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.PL => results.PL?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.PS => results.PS?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.PT => results.PT?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.PY => results.PY?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.QA => results.QA?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.RO => results.RO?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.RS => results.RS?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.RU => results.RU?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.SA => results.SA?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.SC => results.SC?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.SE => results.SE?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.SG => results.SG?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.SI => results.SI?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.SK => results.SK?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.SM => results.SM?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.SN => results.SN?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.SV => results.SV?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.TC => results.TC?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.TH => results.TH?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.TN => results.TN?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.TR => results.TR?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.TT => results.TT?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.TW => results.TW?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.TZ => results.TZ?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.UG => results.UG?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.US => results.US?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.UY => results.UY?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.VA => results.VA?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.VE => results.VE?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.XK => results.XK?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.YE => results.YE?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.ZA => results.ZA?.free.Cast<ProviderBase>().ToList() ?? [],
            Country.ZM => results.ZM?.free.Cast<ProviderBase>().ToList() ?? [],
            _ => []
        };
    }

    public IEnumerable<ProviderBase> GetAdsListProviders(Country? region)
    {
        if (results == null) return [];

        return region switch
        {
            Country.AD => results.AD?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.AE => results.AE?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.AG => results.AG?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.AL => results.AL?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.AR => results.AR?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.AT => results.AT?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.AU => results.AU?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.BA => results.BA?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.BB => results.BB?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.BE => results.BE?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.BG => results.BG?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.BH => results.BH?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.BM => results.BM?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.BO => results.BO?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.BR => results.BR?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.BS => results.BS?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.CA => results.CA?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.CH => results.CH?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.CI => results.CI?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.CL => results.CL?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.CO => results.CO?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.CR => results.CR?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.CU => results.CU?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.CV => results.CV?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.CZ => results.CZ?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.DE => results.DE?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.DK => results.DK?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.DO => results.DO?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.DZ => results.DZ?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.EC => results.EC?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.EE => results.EE?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.EG => results.EG?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.ES => results.ES?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.FI => results.FI?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.FJ => results.FJ?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.FR => results.FR?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.GB => results.GB?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.GF => results.GF?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.GH => results.GH?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.GI => results.GI?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.GP => results.GP?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.GQ => results.GQ?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.GR => results.GR?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.GT => results.GT?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.HK => results.HK?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.HN => results.HN?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.HR => results.HR?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.HU => results.HU?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.ID => results.ID?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.IE => results.IE?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.IL => results.IL?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.IN => results.IN?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.IQ => results.IQ?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.IS => results.IS?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.IT => results.IT?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.JM => results.JM?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.JO => results.JO?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.JP => results.JP?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.KE => results.KE?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.KR => results.KR?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.KW => results.KW?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.LB => results.LB?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.LC => results.LC?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.LI => results.LI?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.LT => results.LT?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.LV => results.LV?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.LY => results.LY?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.MA => results.MA?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.MC => results.MC?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.MD => results.MD?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.MK => results.MK?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.MT => results.MT?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.MU => results.MU?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.MX => results.MX?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.MY => results.MY?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.MZ => results.MZ?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.NE => results.NE?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.NG => results.NG?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.NL => results.NL?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.NO => results.NO?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.NZ => results.NZ?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.OM => results.OM?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.PA => results.PA?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.PE => results.PE?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.PF => results.PF?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.PH => results.PH?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.PK => results.PK?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.PL => results.PL?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.PS => results.PS?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.PT => results.PT?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.PY => results.PY?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.QA => results.QA?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.RO => results.RO?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.RS => results.RS?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.RU => results.RU?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.SA => results.SA?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.SC => results.SC?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.SE => results.SE?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.SG => results.SG?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.SI => results.SI?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.SK => results.SK?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.SM => results.SM?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.SN => results.SN?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.SV => results.SV?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.TC => results.TC?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.TH => results.TH?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.TN => results.TN?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.TR => results.TR?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.TT => results.TT?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.TW => results.TW?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.TZ => results.TZ?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.UG => results.UG?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.US => results.US?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.UY => results.UY?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.VA => results.VA?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.VE => results.VE?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.XK => results.XK?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.YE => results.YE?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.ZA => results.ZA?.ads.Cast<ProviderBase>().ToList() ?? [],
            Country.ZM => results.ZM?.ads.Cast<ProviderBase>().ToList() ?? [],
            _ => []
        };
    }

    public IEnumerable<ProviderBase> GetFlatRateListProviders(Country? region)
    {
        if (results == null) return [];

        return region switch
        {
            Country.AD => results.AD?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.AE => results.AE?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.AG => results.AG?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.AL => results.AL?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.AR => results.AR?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.AT => results.AT?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.AU => results.AU?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.BA => results.BA?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.BB => results.BB?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.BE => results.BE?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.BG => results.BG?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.BH => results.BH?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.BM => results.BM?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.BO => results.BO?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.BR => results.BR?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.BS => results.BS?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.CA => results.CA?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.CH => results.CH?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.CI => results.CI?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.CL => results.CL?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.CO => results.CO?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.CR => results.CR?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.CU => results.CU?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.CV => results.CV?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.CZ => results.CZ?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.DE => results.DE?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.DK => results.DK?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.DO => results.DO?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.DZ => results.DZ?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.EC => results.EC?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.EE => results.EE?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.EG => results.EG?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.ES => results.ES?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.FI => results.FI?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.FJ => results.FJ?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.FR => results.FR?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.GB => results.GB?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.GF => results.GF?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.GH => results.GH?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.GI => results.GI?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.GP => results.GP?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.GQ => results.GQ?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.GR => results.GR?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.GT => results.GT?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.HK => results.HK?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.HN => results.HN?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.HR => results.HR?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.HU => results.HU?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.ID => results.ID?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.IE => results.IE?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.IL => results.IL?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.IN => results.IN?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.IQ => results.IQ?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.IS => results.IS?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.IT => results.IT?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.JM => results.JM?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.JO => results.JO?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.JP => results.JP?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.KE => results.KE?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.KR => results.KR?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.KW => results.KW?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.LB => results.LB?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.LC => results.LC?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.LI => results.LI?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.LT => results.LT?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.LV => results.LV?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.LY => results.LY?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.MA => results.MA?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.MC => results.MC?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.MD => results.MD?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.MK => results.MK?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.MT => results.MT?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.MU => results.MU?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.MX => results.MX?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.MY => results.MY?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.MZ => results.MZ?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.NE => results.NE?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.NG => results.NG?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.NL => results.NL?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.NO => results.NO?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.NZ => results.NZ?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.OM => results.OM?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.PA => results.PA?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.PE => results.PE?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.PF => results.PF?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.PH => results.PH?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.PK => results.PK?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.PL => results.PL?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.PS => results.PS?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.PT => results.PT?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.PY => results.PY?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.QA => results.QA?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.RO => results.RO?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.RS => results.RS?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.RU => results.RU?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.SA => results.SA?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.SC => results.SC?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.SE => results.SE?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.SG => results.SG?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.SI => results.SI?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.SK => results.SK?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.SM => results.SM?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.SN => results.SN?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.SV => results.SV?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.TC => results.TC?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.TH => results.TH?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.TN => results.TN?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.TR => results.TR?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.TT => results.TT?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.TW => results.TW?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.TZ => results.TZ?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.UG => results.UG?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.US => results.US?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.UY => results.UY?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.VA => results.VA?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.VE => results.VE?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.XK => results.XK?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.YE => results.YE?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.ZA => results.ZA?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            Country.ZM => results.ZM?.flatrate.Cast<ProviderBase>().ToList() ?? [],
            _ => []
        };
    }

    //public IEnumerable<ProviderBase> GetFlatRateBuyListProviders(Region? region)
    //{
    //    if (results == null) return [];

    //    return region switch
    //    {
    //        Region.AD => results.AD?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.AE => results.AE?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.AG => results.AG?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.AL => results.AL?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.AR => results.AR?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.AT => results.AT?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.AU => results.AU?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.BA => results.BA?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.BB => results.BB?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.BE => results.BE?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.BG => results.BG?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.BH => results.BH?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.BM => results.BM?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.BO => results.BO?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.BR => results.BR?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.BS => results.BS?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.CA => results.CA?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.CH => results.CH?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.CI => results.CI?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.CL => results.CL?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.CO => results.CO?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.CR => results.CR?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.CU => results.CU?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.CV => results.CV?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.CZ => results.CZ?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.DE => results.DE?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.DK => results.DK?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.DO => results.DO?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.DZ => results.DZ?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.EC => results.EC?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.EE => results.EE?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.EG => results.EG?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.ES => results.ES?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.FI => results.FI?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.FJ => results.FJ?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.FR => results.FR?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.GB => results.GB?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.GF => results.GF?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.GH => results.GH?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.GI => results.GI?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.GP => results.GP?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.GQ => results.GQ?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.GR => results.GR?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.GT => results.GT?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.HK => results.HK?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.HN => results.HN?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.HR => results.HR?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.HU => results.HU?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.ID => results.ID?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.IE => results.IE?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.IL => results.IL?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.IN => results.IN?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.IQ => results.IQ?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.IS => results.IS?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.IT => results.IT?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.JM => results.JM?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.JO => results.JO?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.JP => results.JP?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.KE => results.KE?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.KR => results.KR?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.KW => results.KW?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.LB => results.LB?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.LC => results.LC?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.LI => results.LI?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.LT => results.LT?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.LV => results.LV?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.LY => results.LY?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.MA => results.MA?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.MC => results.MC?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.MD => results.MD?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.MK => results.MK?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.MT => results.MT?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.MU => results.MU?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.MX => results.MX?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.MY => results.MY?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.MZ => results.MZ?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.NE => results.NE?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.NG => results.NG?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.NL => results.NL?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.NO => results.NO?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.NZ => results.NZ?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.OM => results.OM?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.PA => results.PA?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.PE => results.PE?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.PF => results.PF?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.PH => results.PH?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.PK => results.PK?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.PL => results.PL?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.PS => results.PS?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.PT => results.PT?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.PY => results.PY?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.QA => results.QA?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.RO => results.RO?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.RS => results.RS?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.RU => results.RU?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.SA => results.SA?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.SC => results.SC?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.SE => results.SE?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.SG => results.SG?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.SI => results.SI?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.SK => results.SK?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.SM => results.SM?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.SN => results.SN?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.SV => results.SV?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.TC => results.TC?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.TH => results.TH?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.TN => results.TN?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.TR => results.TR?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.TT => results.TT?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.TW => results.TW?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.TZ => results.TZ?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.UG => results.UG?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.US => results.US?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.UY => results.UY?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.VA => results.VA?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.VE => results.VE?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.XK => results.XK?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.YE => results.YE?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.ZA => results.ZA?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        Region.ZM => results.ZM?.flatrate_and_buy.Cast<ProviderBase>().ToList() ?? [],
    //        _ => []
    //    };
    //}

    public IEnumerable<ProviderBase> GetRentListProviders(Country? region)
    {
        if (results == null) return [];

        return region switch
        {
            Country.AD => results.AD?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.AE => results.AE?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.AG => results.AG?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.AL => results.AL?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.AR => results.AR?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.AT => results.AT?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.AU => results.AU?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.BA => results.BA?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.BB => results.BB?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.BE => results.BE?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.BG => results.BG?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.BH => results.BH?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.BM => results.BM?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.BO => results.BO?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.BR => results.BR?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.BS => results.BS?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.CA => results.CA?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.CH => results.CH?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.CI => results.CI?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.CL => results.CL?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.CO => results.CO?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.CR => results.CR?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.CU => results.CU?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.CV => results.CV?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.CZ => results.CZ?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.DE => results.DE?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.DK => results.DK?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.DO => results.DO?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.DZ => results.DZ?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.EC => results.EC?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.EE => results.EE?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.EG => results.EG?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.ES => results.ES?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.FI => results.FI?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.FJ => results.FJ?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.FR => results.FR?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.GB => results.GB?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.GF => results.GF?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.GH => results.GH?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.GI => results.GI?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.GP => results.GP?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.GQ => results.GQ?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.GR => results.GR?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.GT => results.GT?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.HK => results.HK?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.HN => results.HN?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.HR => results.HR?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.HU => results.HU?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.ID => results.ID?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.IE => results.IE?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.IL => results.IL?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.IN => results.IN?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.IQ => results.IQ?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.IS => results.IS?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.IT => results.IT?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.JM => results.JM?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.JO => results.JO?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.JP => results.JP?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.KE => results.KE?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.KR => results.KR?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.KW => results.KW?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.LB => results.LB?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.LC => results.LC?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.LI => results.LI?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.LT => results.LT?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.LV => results.LV?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.LY => results.LY?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.MA => results.MA?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.MC => results.MC?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.MD => results.MD?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.MK => results.MK?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.MT => results.MT?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.MU => results.MU?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.MX => results.MX?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.MY => results.MY?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.MZ => results.MZ?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.NE => results.NE?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.NG => results.NG?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.NL => results.NL?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.NO => results.NO?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.NZ => results.NZ?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.OM => results.OM?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.PA => results.PA?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.PE => results.PE?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.PF => results.PF?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.PH => results.PH?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.PK => results.PK?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.PL => results.PL?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.PS => results.PS?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.PT => results.PT?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.PY => results.PY?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.QA => results.QA?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.RO => results.RO?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.RS => results.RS?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.RU => results.RU?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.SA => results.SA?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.SC => results.SC?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.SE => results.SE?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.SG => results.SG?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.SI => results.SI?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.SK => results.SK?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.SM => results.SM?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.SN => results.SN?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.SV => results.SV?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.TC => results.TC?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.TH => results.TH?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.TN => results.TN?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.TR => results.TR?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.TT => results.TT?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.TW => results.TW?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.TZ => results.TZ?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.UG => results.UG?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.US => results.US?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.UY => results.UY?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.VA => results.VA?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.VE => results.VE?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.XK => results.XK?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.YE => results.YE?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.ZA => results.ZA?.rent.Cast<ProviderBase>().ToList() ?? [],
            Country.ZM => results.ZM?.rent.Cast<ProviderBase>().ToList() ?? [],
            _ => []
        };
    }

    public IEnumerable<ProviderBase> GetBuyListProviders(Country? region)
    {
        if (results == null) return [];

        return region switch
        {
            Country.AD => results.AD?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.AE => results.AE?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.AG => results.AG?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.AL => results.AL?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.AR => results.AR?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.AT => results.AT?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.AU => results.AU?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.BA => results.BA?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.BB => results.BB?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.BE => results.BE?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.BG => results.BG?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.BH => results.BH?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.BM => results.BM?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.BO => results.BO?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.BR => results.BR?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.BS => results.BS?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.CA => results.CA?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.CH => results.CH?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.CI => results.CI?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.CL => results.CL?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.CO => results.CO?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.CR => results.CR?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.CU => results.CU?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.CV => results.CV?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.CZ => results.CZ?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.DE => results.DE?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.DK => results.DK?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.DO => results.DO?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.DZ => results.DZ?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.EC => results.EC?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.EE => results.EE?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.EG => results.EG?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.ES => results.ES?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.FI => results.FI?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.FJ => results.FJ?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.FR => results.FR?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.GB => results.GB?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.GF => results.GF?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.GH => results.GH?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.GI => results.GI?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.GP => results.GP?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.GQ => results.GQ?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.GR => results.GR?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.GT => results.GT?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.HK => results.HK?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.HN => results.HN?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.HR => results.HR?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.HU => results.HU?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.ID => results.ID?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.IE => results.IE?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.IL => results.IL?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.IN => results.IN?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.IQ => results.IQ?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.IS => results.IS?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.IT => results.IT?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.JM => results.JM?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.JO => results.JO?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.JP => results.JP?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.KE => results.KE?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.KR => results.KR?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.KW => results.KW?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.LB => results.LB?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.LC => results.LC?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.LI => results.LI?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.LT => results.LT?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.LV => results.LV?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.LY => results.LY?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.MA => results.MA?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.MC => results.MC?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.MD => results.MD?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.MK => results.MK?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.MT => results.MT?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.MU => results.MU?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.MX => results.MX?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.MY => results.MY?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.MZ => results.MZ?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.NE => results.NE?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.NG => results.NG?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.NL => results.NL?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.NO => results.NO?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.NZ => results.NZ?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.OM => results.OM?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.PA => results.PA?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.PE => results.PE?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.PF => results.PF?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.PH => results.PH?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.PK => results.PK?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.PL => results.PL?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.PS => results.PS?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.PT => results.PT?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.PY => results.PY?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.QA => results.QA?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.RO => results.RO?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.RS => results.RS?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.RU => results.RU?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.SA => results.SA?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.SC => results.SC?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.SE => results.SE?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.SG => results.SG?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.SI => results.SI?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.SK => results.SK?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.SM => results.SM?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.SN => results.SN?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.SV => results.SV?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.TC => results.TC?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.TH => results.TH?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.TN => results.TN?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.TR => results.TR?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.TT => results.TT?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.TW => results.TW?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.TZ => results.TZ?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.UG => results.UG?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.US => results.US?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.UY => results.UY?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.VA => results.VA?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.VE => results.VE?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.XK => results.XK?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.YE => results.YE?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.ZA => results.ZA?.buy.Cast<ProviderBase>().ToList() ?? [],
            Country.ZM => results.ZM?.buy.Cast<ProviderBase>().ToList() ?? [],
            _ => []
        };
    }
}

public class TMDB_AllProviders
{
    public List<ProviderBase> results { get; set; } = [];
}