namespace Identity.Models.Profile
{
    public class PatientViewModel : IUserViewModel
    {
        public CommonUserViewModel CommonUserViewModel { get; set; } = new CommonUserViewModel();
    }
}