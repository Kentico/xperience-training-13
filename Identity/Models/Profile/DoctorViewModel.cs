namespace Identity.Models.Profile
{
    public class DoctorViewModel : IUserViewModel
    {
        public CommonUserViewModel CommonUserViewModel { get; set; } = new CommonUserViewModel();
    }
}