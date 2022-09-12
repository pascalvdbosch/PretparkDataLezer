using OrigineelDataLezer;
// using FunctioneelDataLezer;

class MainKlasse {
    private static int Engheid(Attractie attractie) => 0; // Opdracht (f): Schrijf de methode Engheid
    public static void Main(string[] args) {
        foreach (var attractie in AttractieDataLezer.Lees().Attracties) {
            Console.WriteLine(attractie.Naam + " uit " + attractie.BouwDatum + " [" + attractie.LengteBeperking + "]");
            Console.WriteLine("Engheidsfactor: " + Engheid(attractie));
        }
    }
}
