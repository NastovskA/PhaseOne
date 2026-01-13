using Microsoft.AspNetCore.Identity;

namespace PhaseOne.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Дополнителни полиња ако сакаш
        // public string FullName { get; set; } = string.Empty;
        public int? TeacherId { get; set; }
        public long? StudentIdRef { get; set; }

    }
}
