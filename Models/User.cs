namespace ArchitectNow.SemanticKernelDemo.Models;

public class UserResults
{
    public List<User> Users { get; set; }
}
public class User
{
    public string NameFirst { get; set; }
    public string NameLast { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}