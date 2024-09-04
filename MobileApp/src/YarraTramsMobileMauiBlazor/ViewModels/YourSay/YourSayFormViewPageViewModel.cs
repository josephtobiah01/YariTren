using Core.Interfaces;
using Core.Models;
using DevExpress.Entity.Model.Metadata;
using System.Runtime.CompilerServices;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Views.YourSay;

namespace YarraTramsMobileMauiBlazor.ViewModels.YourSay
{
    public class YourSayFormViewPageViewModel : ViewModelBase
    {
        #region Fields

        private readonly INavigationService _navigationService;
        private readonly IUserProfileService _userProfileService;
        private readonly IYourSayService _yourSayService;
        private List<Route>? _journeyRoutes;
        private List<string>? _travelDirections;
        private string? _attachPhotoName;
        private Route? _selectedRoute;
        private string? _selectedDirection;
        private bool _travelExperienceYes;
        private bool _travelExperienceNo;
        private bool _isVisibleTravelExperience;
        private bool _isVisibleResponseFeedback;
        private bool _responseFeedbackYes;
        private bool _responseFeedbackNo;
        //private Core.Models.YourSay _yourSay;


        #endregion Fields


        public bool TravelExperienceYes 
        {
            get { return _travelExperienceYes; }
            set { SetPropertyValue(ref _travelExperienceYes, value); }
        }
        public bool TravelExperienceNo 
        {
            get { return _travelExperienceNo; }
            set { SetPropertyValue(ref _travelExperienceNo, value); }
        }
        public bool ResponseFeedbackYes 
        {
            get { return _responseFeedbackYes; }
            set { SetPropertyValue(ref _responseFeedbackYes, value); }
        }
        public bool ResponseFeedbackNo 
        { 
            get { return _responseFeedbackNo; }
            set { SetPropertyValue(ref _responseFeedbackNo, value); }
        }
        public bool IsVisibleResponseFeedback 
        { 
            get { return _isVisibleResponseFeedback; }
            set { SetPropertyValue(ref _isVisibleResponseFeedback, value); }
        }
        public bool IsVisibleTravelExperience 
        { 
            get { return  _isVisibleTravelExperience; }
            set { SetPropertyValue(ref _isVisibleTravelExperience, value); }
        }
        public List<Route>? JourneyRoutes
        {
            get { return _journeyRoutes; }
            set { SetPropertyValue(ref _journeyRoutes, value); }
        }
        public List<string>? TravelDirections
        {
            get { return _travelDirections; }
            set { SetPropertyValue(ref _travelDirections, value); }
        }
        public TimeSpan TimeExperience { get; set; }
        //public IUserDialogs _dialogs { get; set; }
        public string? AttachPhotoName
        {
            get { return _attachPhotoName; }
            set { SetPropertyValue(ref _attachPhotoName, value); }
        }

        public bool IsSubmitEnable { get; set; }
        public Route? SelectedRoute
        {
            get { return _selectedRoute; }
            set { SetPropertyValue(ref _selectedRoute, value); }
        }
        public string? SelectedDirection
        {
            get { return _selectedDirection; }
            set { SetPropertyValue(ref _selectedDirection, value); }
        }

        public Core.Models.YourSay YourSay { get; set; }
        public string FormType { get; set; } = "Feedback";
        public string OverviewText { get; set; } = "Your feedback plays an important role in improving the delivery of Yarra Trams services.";

        public Command Submit { get; set; }
        public Command GoBack { get; set; }

        #region Constructor

        public YourSayFormViewPageViewModel(
            INavigationService navigationService,
            IUserProfileService userProfileService,
            IYourSayService yourSayService) : base(navigationService)
        {
            _navigationService = navigationService;
            _userProfileService = userProfileService;
            _yourSayService = yourSayService;

            YourSay = new Core.Models.YourSay();

            Initialize();


            Submit = new Command(SubmitCommand);
            GoBack = new Command(GoBackCommand);

            
        }

        private void GoBackCommand(object obj)
        {
            var navigationStack = Shell.Current.Navigation.NavigationStack;
            var yourSayFormViewPage = navigationStack.FirstOrDefault(p => p is YourSayFormViewPage);

            //await _navigationService.GoBack();
            //await Shell.Current.GoToAsync("..//YourSayViewPage", true);
            Shell.Current.Navigation.RemovePage(yourSayFormViewPage);
            
        }




        #endregion Constructor

        #region Methods

        public override async Task OnNavigatingTo(Dictionary<string, object> parameter)
        {
            
            if (parameter.TryGetValue("FormType", out var formTypeObject) && formTypeObject is string formType)
            {
                FormType = formType;
            }
            //if (parameter.TryGetValue("YourSayFormViewPageTitle", out var YourSayFormViewPageTitle) && YourSayFormViewPageTitle is string)
            //{
                
            //}

            await base.OnNavigatingTo(parameter);

        }

        private async void Initialize()
        {

            UserProfile userProfile = new UserProfile();

            IsVisibleResponseFeedback = false;
            IsVisibleTravelExperience = false;
            IsSubmitEnable = false;

            JourneyRoutes = _yourSayService.LoadJourneyRoutes();
            TravelDirections = _yourSayService.GetTravelDirections();

            YourSay.ExperienceDate = DateTime.Now;
            TimeExperience = YourSay.ExperienceDate.TimeOfDay;
            AttachPhotoName = "Attach photo (optional)";
            

            userProfile = await _userProfileService.GetCurrentUserProfile();

            YourSay.Email = userProfile.Email;
            YourSay.MobileNumber = userProfile.MobileNumber;

        }

        public Command AttachPhotoCommand => new Command(async () => await ShowMediaActionSheet());

        private async Task ShowMediaActionSheet()
        {
            try
            {
                var result = await ShowActionSheetAsync(
                    "Attach Photo",
                    "Cancel",
                    "",
                    "Take Photo",
                    "Select Photo");

                switch (result)
                {
                    case "Take Photo":
                        await TakePhoto();
                        break;
                    case "Select Photo":
                        await SelectPhoto();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) 
            {
                await ShowAlertAsync("Error", $"{ex.Message}");
            }
        }

        private async Task<bool> TakePhoto()
        {
            try
            {
                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Photos>();

                if (status == PermissionStatus.Denied || status == PermissionStatus.Unknown || status == PermissionStatus.Disabled)
                {

                    status = await Permissions.RequestAsync<Permissions.Photos>();

                    if (status == PermissionStatus.Denied || status == PermissionStatus.Unknown || status == PermissionStatus.Disabled)
                    {
                        await ShowAlertAsync("Required", "You must allow the permission to get your image", "Ok");
                        return false;
                    }
                }

                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null)
                    return false;

                var photoName = string.Format("YarraTrams_photo_{0}_{1}_{2}_{3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Minute);
                var newFile = Path.Combine(FileSystem.CacheDirectory, photoName);

                using (var stream = await photo.OpenReadAsync())
                using (var newStream = File.OpenWrite(newFile))
                    await stream.CopyToAsync(newStream);

                FileInfo info = new FileInfo(newFile);
                long size = info.Length;

                if (size <= 8388608)
                {
                    byte[] photoBytes;
                    using (var stream = await photo.OpenReadAsync())
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        photoBytes = ms.ToArray();
                    }

                    YourSay.Photo = photoBytes;
                    AttachPhotoName = photoName;

                    return true;
                }
                else
                {
                    await ShowAlertAsync("File Size Exceeded", "Please take a photo with a maximum file size of 8 MB.", "OK");

                    return false;
                }
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Error", $"An error occurred: {ex.Message}", "OK");

                return false;
            }
        }


        private async Task<bool> SelectPhoto()
        {

            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Photos>();
                }

                if (status != PermissionStatus.Granted)
                {
                    await ShowAlertAsync("Permissions Denied", "Unable to access photos.", "OK");
                    return false;
                }

                var photo = await MediaPicker.PickPhotoAsync();
                if (photo == null)
                    return false;

                var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using (var stream = await photo.OpenReadAsync())
                using (var newStream = File.OpenWrite(newFile))
                    await stream.CopyToAsync(newStream);

                FileInfo info = new FileInfo(newFile);
                long size = info.Length;
                if (size <= 8388608)
                {
                    byte[] photoBytes;
                    using (var stream = await photo.OpenReadAsync())
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        photoBytes = ms.ToArray();
                    }

                    YourSay.Photo = photoBytes;
                    AttachPhotoName = photo.FileName;
                    return true;
                }
                else
                {
                    await ShowAlertAsync("File Size Exceeded", "Please attach a photo with a maximum file size of 8 MB.", "OK");
                    return false;
                }
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Error", $"An error occurred: {ex.Message}", "OK");
                return false;
            }
        }



        public Command TravelExperienceCommandYes
        {
            get
            {
                return new Command(() =>
                {
                    try
                    {
                        if (TravelExperienceYes)
                        {
                            TravelExperienceYes = false;
                            IsVisibleTravelExperience = false;
                        }
                        else
                        {
                            TravelExperienceYes = true;
                            IsVisibleTravelExperience = true;
                            TravelExperienceNo = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowAlert("Error: ", $"{ex.Message}");
                    }
                });
            }
        }

        public Command TravelExperienceCommandNo
        {
            get
            {
                return new Command(() =>
                {
                    try
                    {
                        if (TravelExperienceNo)
                        {
                            TravelExperienceNo = false;
                        }
                        else
                        {
                            TravelExperienceYes = false;
                            IsVisibleTravelExperience = false;
                            TravelExperienceNo = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowAlert("Error: ", $"{ex.Message}");
                    }
                });
            }
        }

        public Command ResponseFeedbackCommandYes
        {
            get
            {
                return new Command(() =>
                {
                    try
                    {
                        if (ResponseFeedbackYes)
                        {
                            ResponseFeedbackYes = false;
                            IsVisibleResponseFeedback = false;

                        }
                        else
                        {
                            ResponseFeedbackYes = true;
                            IsVisibleResponseFeedback = true;
                            ResponseFeedbackNo = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowAlert("Error: ", $"{ex.Message}");
                    }
                });
            }
        }

        public Command ResponseFeedbackCommandNo
        {
            get
            {
                return new Command(() =>
                {
                    try
                    {
                        if (ResponseFeedbackNo)
                        {
                            ResponseFeedbackNo = false;
                        }
                        else
                        {
                            ResponseFeedbackYes = false;
                            IsVisibleResponseFeedback = false;
                            ResponseFeedbackNo = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowAlert("Error: ", $"{ex.Message}");
                    }
                });
            }
        }

        //public Command SubmitCommand
        //{
        //    //get
        //    //{
        //    //    return new Command(async () =>
        //    //    {

        //    //    });
        //    //}


        //}

        private async void SubmitCommand(object obj)
        {
            try
            {
                var response = FormValidationMesage();
                if (response == string.Empty)
                {
                    // Attempt to save. The following is intended to show a dialog, submit the form, hide the main nav page while we are poppping pages, prior to the form submit page being shown...
                    //_dialogs.ShowLoading("Submitting...", MaskType.Black);
                    //MainPage.Instance.ShowLoading();

                    System.Threading.Thread.Sleep(100);
                    bool success = await SaveYourSayFormsAsync();
                    if (success)
                    {
                        if (YourSay.HasAttachment) // check for attachements - if so attempt to upload
                        {
                            bool attachmentSaved = await UploadAttachmentAsync();
                            if (attachmentSaved)
                            {
                                await NavigateToConfirmationPage();

                            }
                            else
                            {
                                ShowAlert("Form was submitted, however the photo/attachment was not uploaded successfully.", "Failed to upload photo");
                            }
                        }
                        else
                        {
                            await NavigateToConfirmationPage();
                        }
                    }
                    else
                    {
                        ShowAlert("Form was not successfully submitted - please try again.", "Failed to submit");
                    }
                }
                else
                {
                    ShowAlert("Sorry", $"{response}");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Failed to submit", $"There was an unexpected error submitting the form - please try again. Error Message: {ex.Message}");
            }
            //finally
            //{
            //    //MainPage.Instance.HideLoading();
            //}
        }

        private async Task NavigateToConfirmationPage()
        {
            //await _navigationService.NavigateTo(nameof(ConfirmationViewPage));
            await Shell.Current.GoToAsync(nameof(ConfirmationViewPage));
        }

        private async Task<bool> SaveYourSayFormsAsync()
        {
            YourSay.FormType = FormType;
            YourSay.ExperienceDate = YourSay.ExperienceDate.Add(TimeExperience);
            YourSay.TravelRelated = TravelExperienceYes;
            if (TravelExperienceYes)
            {
                if (SelectedRoute != null)
                {
                    YourSay.Route = SelectedRoute.DisplayName;
                }
                if (!string.IsNullOrEmpty(SelectedDirection))
                {
                    YourSay.TravelDirection = SelectedDirection;
                }
            }
            YourSay.ContactUser = ResponseFeedbackYes;
            if (!ResponseFeedbackYes)
            {
                YourSay.Email = string.Empty;
                YourSay.MobileNumber = string.Empty;
            }

            // Updated to run as background task...so as to reduce load on main thread and improve responsiveness
            bool success = await _yourSayService.SaveYourSay(YourSay);
            return success;
        }


        private string FormValidationMesage()
        {
            string validationMessage = "";
            if (string.IsNullOrEmpty(YourSay.Title))
            {
                validationMessage += "Feedback subject is required\n";
            }
            if (string.IsNullOrEmpty(YourSay.FeedbackMessage))
            {
                validationMessage += "Feedback Message is required\n";
            }
            if (TravelExperienceYes && FormType != SelectionState.MakeSuggestion.ToString())
            {
                if (SelectedRoute == null)
                {
                    validationMessage += "Journey route is required\n";
                }
                if (string.IsNullOrEmpty(YourSay.JourneyFrom))
                {
                    validationMessage += "Journey from is required\n";
                }
                if (string.IsNullOrEmpty(YourSay.JourneyTo))
                {
                    validationMessage += "Journey to is required\n";
                }

                if (string.IsNullOrEmpty(SelectedDirection))
                {
                    validationMessage += "Travel direction is required\n";
                }
            }
            if (ResponseFeedbackYes)
            {
                if (string.IsNullOrEmpty(YourSay.Email) && string.IsNullOrEmpty(YourSay.MobileNumber))
                {
                    validationMessage += "An Email or Mobile number is required\n";
                }
            }
            return validationMessage;
        }


        private async Task<bool> UploadAttachmentAsync()
        {
            bool success = false;
            if (YourSay.HasAttachment)
            {
                if (!Path.HasExtension(AttachPhotoName))
                {
                    AttachPhotoName = AttachPhotoName + ".jpg";
                }
                YourSay.PhotoName = AttachPhotoName;
                // This might take a while - run it on a background thread and let the UI continue
                return await _yourSayService.SaveYourSayAttachment(YourSay);
            }
            else
            {
                // if not attachment, dont need to do anything - return true;
                success = true;
            }
            return success;
        }

        #endregion Methods
    }
}
