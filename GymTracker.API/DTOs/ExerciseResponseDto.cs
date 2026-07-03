public class ExerciseResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string MuscleGroup { get; set; } = "";

    public string Equipment { get; set; } = "";

    public string Description { get; set; } = "";

    public string Difficulty { get; set; } = "";

    public bool IsCompound { get; set; }

    public string VideoUrl { get; set; } = "";
}