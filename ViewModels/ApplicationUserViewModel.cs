public class UserViewModel
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsAdmin { get; set; }

    // Computed property for display
    public string FullName => $"{FirstName} {LastName}";
}
