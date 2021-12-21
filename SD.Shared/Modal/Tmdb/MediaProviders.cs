using SD.Shared.Modal.Enum;
using System.Collections.Generic;
using System.Linq;

namespace SD.Shared.Modal.Tmdb
{
    public class ProviderBase
    {
        public int display_priority { get; set; }
        public string logo_path { get; set; }
        public int provider_id { get; set; }
        public string provider_name { get; set; }
        public string provider_desciption { get; set; }
        public string provider_link { get; set; }
    }

    public class Free : ProviderBase { }

    public class Ads : ProviderBase { }

    public class Flatrate : ProviderBase { }

    public class FlatrateAndBuy : ProviderBase { }

    public class Rent : ProviderBase { }

    public class Buy : ProviderBase { }

    public class CountryBase
    {
        public string link { get; set; }
        public List<Free> free { get; set; } = new();
        public List<Ads> ads { get; set; } = new();
        public List<Flatrate> flatrate { get; set; } = new();
        public List<FlatrateAndBuy> flatrate_and_buy { get; set; } = new();
        public List<Rent> rent { get; set; } = new();
        public List<Buy> buy { get; set; } = new();
    }

    public class AR : CountryBase { }

    public class AT : CountryBase { }

    public class AU : CountryBase { }

    public class BE : CountryBase { }

    public class BG : CountryBase { }

    public class BR : CountryBase { }

    public class CA : CountryBase { }

    public class CH : CountryBase { }

    public class CL : CountryBase { }

    public class CO : CountryBase { }

    public class CZ : CountryBase { }

    public class DE : CountryBase { }

    public class DK : CountryBase { }

    public class EC : CountryBase { }

    public class EE : CountryBase { }

    public class ES : CountryBase { }

    public class FI : CountryBase { }

    public class FR : CountryBase { }

    public class GB : CountryBase { }

    public class GR : CountryBase { }

    public class HU : CountryBase { }

    public class ID : CountryBase { }

    public class IE : CountryBase { }

    public class IN : CountryBase { }

    public class IT : CountryBase { }

    public class JP : CountryBase { }

    public class KR : CountryBase { }

    public class LT : CountryBase { }

    public class LV : CountryBase { }

    public class MX : CountryBase { }

    public class MY : CountryBase { }

    public class NL : CountryBase { }

    public class NO : CountryBase { }

    public class NZ : CountryBase { }

    public class PE : CountryBase { }

    public class PH : CountryBase { }

    public class PL : CountryBase { }

    public class PT : CountryBase { }

    public class RO : CountryBase { }

    public class RU : CountryBase { }

    public class SE : CountryBase { }

    public class SG : CountryBase { }

    public class TH : CountryBase { }

    public class TR : CountryBase { }

    public class US : CountryBase { }

    public class VE : CountryBase { }

    public class ZA : CountryBase { }

    public class Results
    {
        public AR AR { get; set; }
        public AT AT { get; set; }
        public AU AU { get; set; }
        public BE BE { get; set; }
        public BG BG { get; set; }
        public BR BR { get; set; }
        public CA CA { get; set; }
        public CH CH { get; set; }
        public CL CL { get; set; }
        public CO CO { get; set; }
        public CZ CZ { get; set; }
        public DE DE { get; set; }
        public DK DK { get; set; }
        public EC EC { get; set; }
        public EE EE { get; set; }
        public ES ES { get; set; }
        public FI FI { get; set; }
        public FR FR { get; set; }
        public GB GB { get; set; }
        public GR GR { get; set; }
        public HU HU { get; set; }
        public ID ID { get; set; }
        public IE IE { get; set; }
        public IN IN { get; set; }
        public IT IT { get; set; }
        public JP JP { get; set; }
        public KR KR { get; set; }
        public LT LT { get; set; }
        public LV LV { get; set; }
        public MX MX { get; set; }
        public MY MY { get; set; }
        public NL NL { get; set; }
        public NO NO { get; set; }
        public NZ NZ { get; set; }
        public PE PE { get; set; }
        public PH PH { get; set; }
        public PL PL { get; set; }
        public PT PT { get; set; }
        public RO RO { get; set; }
        public RU RU { get; set; }
        public SE SE { get; set; }
        public SG SG { get; set; }
        public TH TH { get; set; }
        public TR TR { get; set; }
        public US US { get; set; }
        public VE VE { get; set; }
        public ZA ZA { get; set; }
    }

    public class MediaProviders
    {
        public int id { get; set; }
        public Results results { get; set; }

        public string GetLink(Region region)
        {
            try
            {
                switch (region)
                {
                    case Region.AR:
                        return results.AR.link;

                    case Region.AT:
                        return results.AT.link;

                    case Region.AU:
                        return results.AU.link;

                    case Region.BE:
                        return results.BE.link;

                    case Region.BG:
                        return results.BG.link;

                    case Region.BR:
                        return results.BR.link;

                    case Region.CA:
                        return results.CA.link;

                    case Region.CH:
                        return results.CH.link;

                    case Region.CZ:
                        return results.CZ.link;

                    case Region.DE:
                        return results.DE.link;

                    case Region.DK:
                        return results.DK.link;

                    case Region.EE:
                        return results.EE.link;

                    case Region.ES:
                        return results.ES.link;

                    case Region.FI:
                        return results.FI.link;

                    case Region.FR:
                        return results.FR.link;

                    case Region.GB:
                        return results.GB.link;

                    case Region.HU:
                        return results.HU.link;

                    case Region.ID:
                        return results.ID.link;

                    case Region.IE:
                        return results.IE.link;

                    case Region.IN:
                        return results.IN.link;

                    case Region.IT:
                        return results.IT.link;

                    case Region.JP:
                        return results.JP.link;

                    case Region.KR:
                        return results.KR.link;

                    case Region.LT:
                        return results.LT.link;

                    case Region.MX:
                        return results.MX.link;

                    case Region.NL:
                        return results.NL.link;

                    case Region.NO:
                        return results.NO.link;

                    case Region.NZ:
                        return results.NZ.link;

                    case Region.PH:
                        return results.PH.link;

                    case Region.PL:
                        return results.PL.link;

                    case Region.PT:
                        return results.PT.link;

                    case Region.RU:
                        return results.RU.link;

                    case Region.SE:
                        return results.SE.link;

                    case Region.TR:
                        return results.TR.link;

                    case Region.US:
                        return results.US.link;

                    case Region.ZA:
                        return results.ZA.link;

                    default:
                        return default;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<ProviderBase> GetFreeListProviders(Region region)
        {
            try
            {
                switch (region)
                {
                    case Region.AR:
                        return results.AR.free.Select(s => (ProviderBase)s);

                    case Region.AT:
                        return results.AT.free.Select(s => (ProviderBase)s);

                    case Region.AU:
                        return results.AU.free.Select(s => (ProviderBase)s);

                    case Region.BE:
                        return results.BE.free.Select(s => (ProviderBase)s);

                    case Region.BG:
                        return results.BG.free.Select(s => (ProviderBase)s);

                    case Region.BR:
                        return results.BR.free.Select(s => (ProviderBase)s);

                    case Region.CA:
                        return results.CA.free.Select(s => (ProviderBase)s);

                    case Region.CH:
                        return results.CH.free.Select(s => (ProviderBase)s);

                    case Region.CZ:
                        return results.CZ.free.Select(s => (ProviderBase)s);

                    case Region.DE:
                        return results.DE.free.Select(s => (ProviderBase)s);

                    case Region.DK:
                        return results.DK.free.Select(s => (ProviderBase)s);

                    case Region.EE:
                        return results.EE.free.Select(s => (ProviderBase)s);

                    case Region.ES:
                        return results.ES.free.Select(s => (ProviderBase)s);

                    case Region.FI:
                        return results.FI.free.Select(s => (ProviderBase)s);

                    case Region.FR:
                        return results.FR.free.Select(s => (ProviderBase)s);

                    case Region.GB:
                        return results.GB.free.Select(s => (ProviderBase)s);

                    case Region.HU:
                        return results.HU.free.Select(s => (ProviderBase)s);

                    case Region.ID:
                        return results.ID.free.Select(s => (ProviderBase)s);

                    case Region.IE:
                        return results.IE.free.Select(s => (ProviderBase)s);

                    case Region.IN:
                        return results.IN.free.Select(s => (ProviderBase)s);

                    case Region.IT:
                        return results.IT.free.Select(s => (ProviderBase)s);

                    case Region.JP:
                        return results.JP.free.Select(s => (ProviderBase)s);

                    case Region.KR:
                        return results.KR.free.Select(s => (ProviderBase)s);

                    case Region.LT:
                        return results.LT.free.Select(s => (ProviderBase)s);

                    case Region.MX:
                        return results.MX.free.Select(s => (ProviderBase)s);

                    case Region.NL:
                        return results.NL.free.Select(s => (ProviderBase)s);

                    case Region.NO:
                        return results.NO.free.Select(s => (ProviderBase)s);

                    case Region.NZ:
                        return results.NZ.free.Select(s => (ProviderBase)s);

                    case Region.PH:
                        return results.PH.free.Select(s => (ProviderBase)s);

                    case Region.PL:
                        return results.PL.free.Select(s => (ProviderBase)s);

                    case Region.PT:
                        return results.PT.free.Select(s => (ProviderBase)s);

                    case Region.RU:
                        return results.RU.free.Select(s => (ProviderBase)s);

                    case Region.SE:
                        return results.SE.free.Select(s => (ProviderBase)s);

                    case Region.TR:
                        return results.TR.free.Select(s => (ProviderBase)s);

                    case Region.US:
                        return results.US.free.Select(s => (ProviderBase)s);

                    case Region.ZA:
                        return results.ZA.free.Select(s => (ProviderBase)s);

                    default:
                        return default;
                }
            }
            catch (Exception)
            {
                return new List<ProviderBase>();
            }
        }

        public IEnumerable<ProviderBase> GetAdsListProviders(Region region)
        {
            try
            {
                switch (region)
                {
                    case Region.AR:
                        return results.AR.ads.Select(s => (ProviderBase)s);

                    case Region.AT:
                        return results.AT.ads.Select(s => (ProviderBase)s);

                    case Region.AU:
                        return results.AU.ads.Select(s => (ProviderBase)s);

                    case Region.BE:
                        return results.BE.ads.Select(s => (ProviderBase)s);

                    case Region.BG:
                        return results.BG.ads.Select(s => (ProviderBase)s);

                    case Region.BR:
                        return results.BR.ads.Select(s => (ProviderBase)s);

                    case Region.CA:
                        return results.CA.ads.Select(s => (ProviderBase)s);

                    case Region.CH:
                        return results.CH.ads.Select(s => (ProviderBase)s);

                    case Region.CZ:
                        return results.CZ.ads.Select(s => (ProviderBase)s);

                    case Region.DE:
                        return results.DE.ads.Select(s => (ProviderBase)s);

                    case Region.DK:
                        return results.DK.ads.Select(s => (ProviderBase)s);

                    case Region.EE:
                        return results.EE.ads.Select(s => (ProviderBase)s);

                    case Region.ES:
                        return results.ES.ads.Select(s => (ProviderBase)s);

                    case Region.FI:
                        return results.FI.ads.Select(s => (ProviderBase)s);

                    case Region.FR:
                        return results.FR.ads.Select(s => (ProviderBase)s);

                    case Region.GB:
                        return results.GB.ads.Select(s => (ProviderBase)s);

                    case Region.HU:
                        return results.HU.ads.Select(s => (ProviderBase)s);

                    case Region.ID:
                        return results.ID.ads.Select(s => (ProviderBase)s);

                    case Region.IE:
                        return results.IE.ads.Select(s => (ProviderBase)s);

                    case Region.IN:
                        return results.IN.ads.Select(s => (ProviderBase)s);

                    case Region.IT:
                        return results.IT.ads.Select(s => (ProviderBase)s);

                    case Region.JP:
                        return results.JP.ads.Select(s => (ProviderBase)s);

                    case Region.KR:
                        return results.KR.ads.Select(s => (ProviderBase)s);

                    case Region.LT:
                        return results.LT.ads.Select(s => (ProviderBase)s);

                    case Region.MX:
                        return results.MX.ads.Select(s => (ProviderBase)s);

                    case Region.NL:
                        return results.NL.ads.Select(s => (ProviderBase)s);

                    case Region.NO:
                        return results.NO.ads.Select(s => (ProviderBase)s);

                    case Region.NZ:
                        return results.NZ.ads.Select(s => (ProviderBase)s);

                    case Region.PH:
                        return results.PH.ads.Select(s => (ProviderBase)s);

                    case Region.PL:
                        return results.PL.ads.Select(s => (ProviderBase)s);

                    case Region.PT:
                        return results.PT.ads.Select(s => (ProviderBase)s);

                    case Region.RU:
                        return results.RU.ads.Select(s => (ProviderBase)s);

                    case Region.SE:
                        return results.SE.ads.Select(s => (ProviderBase)s);

                    case Region.TR:
                        return results.TR.ads.Select(s => (ProviderBase)s);

                    case Region.US:
                        return results.US.ads.Select(s => (ProviderBase)s);

                    case Region.ZA:
                        return results.ZA.ads.Select(s => (ProviderBase)s);

                    default:
                        return default;
                }
            }
            catch (Exception)
            {
                return new List<ProviderBase>();
            }
        }

        public IEnumerable<ProviderBase> GetFlatRateListProviders(Region region)
        {
            try
            {
                switch (region)
                {
                    case Region.AR:
                        return results.AR.flatrate.Select(s => (ProviderBase)s);

                    case Region.AT:
                        return results.AT.flatrate.Select(s => (ProviderBase)s);

                    case Region.AU:
                        return results.AU.flatrate.Select(s => (ProviderBase)s);

                    case Region.BE:
                        return results.BE.flatrate.Select(s => (ProviderBase)s);

                    case Region.BG:
                        return results.BG.flatrate.Select(s => (ProviderBase)s);

                    case Region.BR:
                        return results.BR.flatrate.Select(s => (ProviderBase)s);

                    case Region.CA:
                        return results.CA.flatrate.Select(s => (ProviderBase)s);

                    case Region.CH:
                        return results.CH.flatrate.Select(s => (ProviderBase)s);

                    case Region.CZ:
                        return results.CZ.flatrate.Select(s => (ProviderBase)s);

                    case Region.DE:
                        return results.DE.flatrate.Select(s => (ProviderBase)s);

                    case Region.DK:
                        return results.DK.flatrate.Select(s => (ProviderBase)s);

                    case Region.EE:
                        return results.EE.flatrate.Select(s => (ProviderBase)s);

                    case Region.ES:
                        return results.ES.flatrate.Select(s => (ProviderBase)s);

                    case Region.FI:
                        return results.FI.flatrate.Select(s => (ProviderBase)s);

                    case Region.FR:
                        return results.FR.flatrate.Select(s => (ProviderBase)s);

                    case Region.GB:
                        return results.GB.flatrate.Select(s => (ProviderBase)s);

                    case Region.HU:
                        return results.HU.flatrate.Select(s => (ProviderBase)s);

                    case Region.ID:
                        return results.ID.flatrate.Select(s => (ProviderBase)s);

                    case Region.IE:
                        return results.IE.flatrate.Select(s => (ProviderBase)s);

                    case Region.IN:
                        return results.IN.flatrate.Select(s => (ProviderBase)s);

                    case Region.IT:
                        return results.IT.flatrate.Select(s => (ProviderBase)s);

                    case Region.JP:
                        return results.JP.flatrate.Select(s => (ProviderBase)s);

                    case Region.KR:
                        return results.KR.flatrate.Select(s => (ProviderBase)s);

                    case Region.LT:
                        return results.LT.flatrate.Select(s => (ProviderBase)s);

                    case Region.MX:
                        return results.MX.flatrate.Select(s => (ProviderBase)s);

                    case Region.NL:
                        return results.NL.flatrate.Select(s => (ProviderBase)s);

                    case Region.NO:
                        return results.NO.flatrate.Select(s => (ProviderBase)s);

                    case Region.NZ:
                        return results.NZ.flatrate.Select(s => (ProviderBase)s);

                    case Region.PH:
                        return results.PH.flatrate.Select(s => (ProviderBase)s);

                    case Region.PL:
                        return results.PL.flatrate.Select(s => (ProviderBase)s);

                    case Region.PT:
                        return results.PT.flatrate.Select(s => (ProviderBase)s);

                    case Region.RU:
                        return results.RU.flatrate.Select(s => (ProviderBase)s);

                    case Region.SE:
                        return results.SE.flatrate.Select(s => (ProviderBase)s);

                    case Region.TR:
                        return results.TR.flatrate.Select(s => (ProviderBase)s);

                    case Region.US:
                        return results.US.flatrate.Select(s => (ProviderBase)s);

                    case Region.ZA:
                        return results.ZA.flatrate.Select(s => (ProviderBase)s);

                    default:
                        return default;
                }
            }
            catch (Exception)
            {
                return new List<ProviderBase>();
            }
        }

        public IEnumerable<ProviderBase> GetFlatRateBuyListProviders(Region region)
        {
            try
            {
                switch (region)
                {
                    case Region.AR:
                        return results.AR.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.AT:
                        return results.AT.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.AU:
                        return results.AU.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.BE:
                        return results.BE.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.BG:
                        return results.BG.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.BR:
                        return results.BR.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.CA:
                        return results.CA.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.CH:
                        return results.CH.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.CZ:
                        return results.CZ.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.DE:
                        return results.DE.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.DK:
                        return results.DK.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.EE:
                        return results.EE.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.ES:
                        return results.ES.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.FI:
                        return results.FI.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.FR:
                        return results.FR.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.GB:
                        return results.GB.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.HU:
                        return results.HU.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.ID:
                        return results.ID.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.IE:
                        return results.IE.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.IN:
                        return results.IN.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.IT:
                        return results.IT.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.JP:
                        return results.JP.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.KR:
                        return results.KR.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.LT:
                        return results.LT.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.MX:
                        return results.MX.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.NL:
                        return results.NL.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.NO:
                        return results.NO.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.NZ:
                        return results.NZ.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.PH:
                        return results.PH.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.PL:
                        return results.PL.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.PT:
                        return results.PT.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.RU:
                        return results.RU.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.SE:
                        return results.SE.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.TR:
                        return results.TR.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.US:
                        return results.US.flatrate_and_buy.Select(s => (ProviderBase)s);

                    case Region.ZA:
                        return results.ZA.flatrate_and_buy.Select(s => (ProviderBase)s);

                    default:
                        return default;
                }
            }
            catch (Exception)
            {
                return new List<ProviderBase>();
            }
        }

        public IEnumerable<ProviderBase> GetRentListProviders(Region region)
        {
            try
            {
                switch (region)
                {
                    case Region.AR:
                        return results.AR.rent.Select(s => (ProviderBase)s);

                    case Region.AT:
                        return results.AT.rent.Select(s => (ProviderBase)s);

                    case Region.AU:
                        return results.AU.rent.Select(s => (ProviderBase)s);

                    case Region.BE:
                        return results.BE.rent.Select(s => (ProviderBase)s);

                    case Region.BG:
                        return results.BG.rent.Select(s => (ProviderBase)s);

                    case Region.BR:
                        return results.BR.rent.Select(s => (ProviderBase)s);

                    case Region.CA:
                        return results.CA.rent.Select(s => (ProviderBase)s);

                    case Region.CH:
                        return results.CH.rent.Select(s => (ProviderBase)s);

                    case Region.CZ:
                        return results.CZ.rent.Select(s => (ProviderBase)s);

                    case Region.DE:
                        return results.DE.rent.Select(s => (ProviderBase)s);

                    case Region.DK:
                        return results.DK.rent.Select(s => (ProviderBase)s);

                    case Region.EE:
                        return results.EE.rent.Select(s => (ProviderBase)s);

                    case Region.ES:
                        return results.ES.rent.Select(s => (ProviderBase)s);

                    case Region.FI:
                        return results.FI.rent.Select(s => (ProviderBase)s);

                    case Region.FR:
                        return results.FR.rent.Select(s => (ProviderBase)s);

                    case Region.GB:
                        return results.GB.rent.Select(s => (ProviderBase)s);

                    case Region.HU:
                        return results.HU.rent.Select(s => (ProviderBase)s);

                    case Region.ID:
                        return results.ID.rent.Select(s => (ProviderBase)s);

                    case Region.IE:
                        return results.IE.rent.Select(s => (ProviderBase)s);

                    case Region.IN:
                        return results.IN.rent.Select(s => (ProviderBase)s);

                    case Region.IT:
                        return results.IT.rent.Select(s => (ProviderBase)s);

                    case Region.JP:
                        return results.JP.rent.Select(s => (ProviderBase)s);

                    case Region.KR:
                        return results.KR.rent.Select(s => (ProviderBase)s);

                    case Region.LT:
                        return results.LT.rent.Select(s => (ProviderBase)s);

                    case Region.MX:
                        return results.MX.rent.Select(s => (ProviderBase)s);

                    case Region.NL:
                        return results.NL.rent.Select(s => (ProviderBase)s);

                    case Region.NO:
                        return results.NO.rent.Select(s => (ProviderBase)s);

                    case Region.NZ:
                        return results.NZ.rent.Select(s => (ProviderBase)s);

                    case Region.PH:
                        return results.PH.rent.Select(s => (ProviderBase)s);

                    case Region.PL:
                        return results.PL.rent.Select(s => (ProviderBase)s);

                    case Region.PT:
                        return results.PT.rent.Select(s => (ProviderBase)s);

                    case Region.RU:
                        return results.RU.rent.Select(s => (ProviderBase)s);

                    case Region.SE:
                        return results.SE.rent.Select(s => (ProviderBase)s);

                    case Region.TR:
                        return results.TR.rent.Select(s => (ProviderBase)s);

                    case Region.US:
                        return results.US.rent.Select(s => (ProviderBase)s);

                    case Region.ZA:
                        return results.ZA.rent.Select(s => (ProviderBase)s);

                    default:
                        return default;
                }
            }
            catch (Exception)
            {
                return new List<ProviderBase>();
            }
        }

        public IEnumerable<ProviderBase> GetBuyListProviders(Region region)
        {
            try
            {
                switch (region)
                {
                    case Region.AR:
                        return results.AR.buy.Select(s => (ProviderBase)s);

                    case Region.AT:
                        return results.AT.buy.Select(s => (ProviderBase)s);

                    case Region.AU:
                        return results.AU.buy.Select(s => (ProviderBase)s);

                    case Region.BE:
                        return results.BE.buy.Select(s => (ProviderBase)s);

                    case Region.BG:
                        return results.BG.buy.Select(s => (ProviderBase)s);

                    case Region.BR:
                        return results.BR.buy.Select(s => (ProviderBase)s);

                    case Region.CA:
                        return results.CA.buy.Select(s => (ProviderBase)s);

                    case Region.CH:
                        return results.CH.buy.Select(s => (ProviderBase)s);

                    case Region.CZ:
                        return results.CZ.buy.Select(s => (ProviderBase)s);

                    case Region.DE:
                        return results.DE.buy.Select(s => (ProviderBase)s);

                    case Region.DK:
                        return results.DK.buy.Select(s => (ProviderBase)s);

                    case Region.EE:
                        return results.EE.buy.Select(s => (ProviderBase)s);

                    case Region.ES:
                        return results.ES.buy.Select(s => (ProviderBase)s);

                    case Region.FI:
                        return results.FI.buy.Select(s => (ProviderBase)s);

                    case Region.FR:
                        return results.FR.buy.Select(s => (ProviderBase)s);

                    case Region.GB:
                        return results.GB.buy.Select(s => (ProviderBase)s);

                    case Region.HU:
                        return results.HU.buy.Select(s => (ProviderBase)s);

                    case Region.ID:
                        return results.ID.buy.Select(s => (ProviderBase)s);

                    case Region.IE:
                        return results.IE.buy.Select(s => (ProviderBase)s);

                    case Region.IN:
                        return results.IN.buy.Select(s => (ProviderBase)s);

                    case Region.IT:
                        return results.IT.buy.Select(s => (ProviderBase)s);

                    case Region.JP:
                        return results.JP.buy.Select(s => (ProviderBase)s);

                    case Region.KR:
                        return results.KR.buy.Select(s => (ProviderBase)s);

                    case Region.LT:
                        return results.LT.buy.Select(s => (ProviderBase)s);

                    case Region.MX:
                        return results.MX.buy.Select(s => (ProviderBase)s);

                    case Region.NL:
                        return results.NL.buy.Select(s => (ProviderBase)s);

                    case Region.NO:
                        return results.NO.buy.Select(s => (ProviderBase)s);

                    case Region.NZ:
                        return results.NZ.buy.Select(s => (ProviderBase)s);

                    case Region.PH:
                        return results.PH.buy.Select(s => (ProviderBase)s);

                    case Region.PL:
                        return results.PL.buy.Select(s => (ProviderBase)s);

                    case Region.PT:
                        return results.PT.buy.Select(s => (ProviderBase)s);

                    case Region.RU:
                        return results.RU.buy.Select(s => (ProviderBase)s);

                    case Region.SE:
                        return results.SE.buy.Select(s => (ProviderBase)s);

                    case Region.TR:
                        return results.TR.buy.Select(s => (ProviderBase)s);

                    case Region.US:
                        return results.US.buy.Select(s => (ProviderBase)s);

                    case Region.ZA:
                        return results.ZA.buy.Select(s => (ProviderBase)s);

                    default:
                        return default;
                }
            }
            catch (Exception)
            {
                return new List<ProviderBase>();
            }
        }
    }

    public class TMDB_AllProviders
    {
        public List<ProviderBase> results { get; set; }
    }
}