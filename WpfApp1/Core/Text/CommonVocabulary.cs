namespace WpfApp1.Core.Text;

public static class CommonVocabulary
{
    private const string Text = """
        the be to of and in that have it for not on with he as you do at this but his by from
        they we say her she or an will my one all would there their what so up out if about who
        get which go me when make can like time no just him know take people into year your good
        some could them see other than then now look only come its over think also back after
        use two how our work first well way even new want because any these give day most us
        man find here thing tell very great big high small large next early young important
        few public bad able world life hand part child eye woman place week case point
        group problem fact number system program question during without again water home room
        mother area money story study book word issue side kind head house service friend
        father power hour game line end member law car city community name president team minute
        idea kid body information parent face level office door health person art war
        history party result change morning reason research girl guy moment air teacher force
        education foot boy age policy process music market sense nation plan college interest
        death experience effect class control care field development role effort rate heart
        drug show leader light voice wife whole police mind report decision son view relationship
        town road arm difference value building action model season society tax director position
        player record paper space ground form event official matter center couple site project
        activity star table need court produce eat teach oil half situation easy cost industry
        figure street image itself phone either data cover quite picture clear practice piece land
        recent describe product doctor wall patient worker news test movie certain north love
        personal open support simply third technology catch step baby computer type attention
        draw film republican tree source red nearly organization choose cause hair
        century evidence window difficult listen soon culture billion chance brother energy
        period course summer less realize hundred available plant likely opportunity term short
        letter condition choice single rule daughter administration south husband floor campaign
        material population economy medical hospital church close thousand risk current fire future
        wrong involve defense anyone increase security bank myself certainly west sport board seek
        subject officer private rest behavior deal performance fight throw top quickly past
        goal second bed order author fill represent focus foreign drop blood upon agency push nature
        color recently store reduce sound note fine before near movement page enter share common
        natural race concern series significant similar hot language usually response dead rise
        animal factor decade article shoot east save seven artist scene stock career despite
        central power eight thus treatment beyond happy exactly protect approach lie size dog
        fund serious occur media ready sign thought list individual simple quality pressure accept
        answer hard resource identify left meeting determine prepare disease whatever success
        argue cup particularly amount ability staff recognize indicate character growth loss degree
        wonder attack herself region television box training pretty trade election everybody physical
        general feeling standard bill message fail outside arrive analysis benefit sell
        guard quarter breath noise rather bill mountain shadow mixture breath storm coin
        bridge bell stream desert dream dust brick silence wave engine harbor valley
        needle thread basket market candle lantern marble copper silver
        journey season winter summer spring autumn morning evening midnight sunrise sunset
        family friend stranger neighbor cousin uncle aunt nephew
        kitchen garden garage attic basement balcony hallway
        orange purple yellow silver golden crimson ivory
        quiet gentle bright dark warm cold heavy light sharp smooth
        answer question reason problem answer solution example pattern shape
        """;

    public static IReadOnlyList<string> Words { get; } =
    [
        .. Text
            .Split([' ', '\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries)
            .Distinct(StringComparer.Ordinal)
    ];
}
