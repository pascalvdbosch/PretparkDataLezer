namespace FunctioneelDataLezer;

using System.Collections.Immutable;


static class Functies
{
    // Opdracht (a): definieer hier de methode Unsplat
    public static Func<T1, T2, T3> Splat<T1, T2, T3>(Func<(T1, T2), T3> f) => (T1 t1, T2 t2) => f((t1, t2));
    public static Func<T1, T3> Combineer<T1, T2, T3>(Func<T2, T3> f2, Func<T1, T2> f1) => (T1 t1) => f2(f1(t1));

    public static Func<TIn, TOutOut> CombineerOutputs<TIn, TOut, TOutOut>(Func<TIn, TOut> f1, Func<TIn, TOut> f2, Func<TOut, TOut, TOutOut> combiner) =>
        (TIn i) => combiner(f1(i), f2(i));
    
    public static Func<T1, (T2, T1)> TellerPrefix<T1, T2>(Func<T2> f) => (T1 t1) => (f(), t1);
}

static class Lezer<T>
{
    public static T[] Lees(string fileName, Func<int, string, T> Functie) => 
        ElkeRegel(
            Filter(
                Functies.Combineer(
                    (bool a) => !a, 
                    Functies.CombineerOutputs<string, bool, bool>(
                        IsLegeRegel, 
                        IsCommentaar, 
                        (bool a, bool b) => a || b
                    )
                ), 
                File.ReadAllLines(fileName)
            ), 
            Functies.Combineer<string, (int, string), T>(
                Functies.Unsplat<int, string, T>(Functie), 
                Functies.TellerPrefix<string, int>(MetNummer())
            )
        );
    
    public static Func<int> MetNummer() {
        // Opdracht (b): definieer MetNummer
    }

    public static string[] Filter(Func<string, bool> f, string[] args) => args switch {
        [] => new string[] {},
        [string s, ..] when f(s) => , // Opdracht (c): wat moet hier komen? (tip: kijk naar ElkeRegel)
        _ => Filter(f, args[1..])
    };

    public static T[] ElkeRegel(string[] regels, Func<string, T> functie) => regels switch {
        [] => new T[] {},
        _ => new T[] {functie(regels[0])}.Concat(ElkeRegel(regels[1..], functie)).ToArray()
    };
    // Opdracht (extra): De methoden Filter en ElkeRegel kunnen abstracter door ze generic te maken, zodat ze voor allerlei soorten lijsten werken (hernoem ElkeRegel naar Map)
    
    private static bool IsLegeRegel(string s) => s.Trim() == "";
    private static bool IsCommentaar(string s) => s.StartsWith("#");
}

readonly record struct LengteBeperking(int? MinimaleLengte, int? MaximaleLengte) {
    public LengteBeperking ZonderMinimum() => this with { MinimaleLengte = null };
    public LengteBeperking ZonderMaximum() => this with { MaximaleLengte = null };
}

// Opdracht (d): schrijf de Attractie klasse

readonly record struct AttractieContext(ImmutableList<Attractie> Attracties)
{
    // Opdracht (e): schrijf de methode NieuweAttractie
}

static class AttractieDataLezer
{
    private static Attractie?[] LeesUitBestand() => Lezer<Attractie?>.Lees("attractie_data.csv", 
        (int i, string regel) => regel.Split(",") switch {
            [string Naam, "Kapot", _, _, _, _] when Naam.StartsWith("") => 
                null,
            [string Naam, _, "Rollercoaster", string BouwDatum, string Lengte, _] => 
                new Attractie.Rollercoaster(Naam, BouwDatum, int.Parse(Lengte)),
            [string Naam, _, "Draaimolen", string BouwDatum, _, string DraaiSnelheid] => 
                new Attractie.Draaimolen(Naam, BouwDatum, int.Parse(DraaiSnelheid)),
            _ => throw new Exception("Leesfout!")
        } );
    private static AttractieContext Verzamel(Attractie?[] attracties) => attracties switch {
        [] => new AttractieContext { Attracties = ImmutableList<Attractie>.Empty }, 
        [null, ..] => Verzamel(attracties[1..]),
        [Attractie a, ..] => Verzamel(attracties[1..]).NieuweAttractie(a)
    };
    public static AttractieContext Lees() => Verzamel(LeesUitBestand());
}

