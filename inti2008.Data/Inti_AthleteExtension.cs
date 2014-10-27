namespace inti2008.Data
{
    public static class Inti_AthleteExtension
    {
        public static string FullName(this IPerson athlete)
        {
            return ((athlete.FirstName ?? "") + " " + (athlete.LastName ?? "")).Trim();
        }
    }

    public partial class Inti_Athlete : IPerson
    {

    }

    public interface IPerson
    {
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}