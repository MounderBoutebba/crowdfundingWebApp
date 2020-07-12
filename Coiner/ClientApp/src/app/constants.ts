export class Constants {
  public static readonly baseUrlServer = "http://localhost:63730/";
  public static readonly sharedImagesPath = "Content/images/";
  public static readonly froalaImagesPath = "Content/froalaImages/";
  public static readonly froalaFilesPath = "Content/froalaFiles/";
  public static readonly froalaVideosPath = "Content/froalaVideos/";

  public static readonly sharedUserImagesPath = "Content/userImages/";
  public static readonly sharedDocumentsPath = "Content/documents/";
  public static readonly BillsSharedPath = "Content/bills/";

  public static readonly FileUploadMaxSize = 10000000;
  public static readonly MaxNumberFiles = 5;
  public static readonly UserCoinsNumber = 100;

  public static readonly AddressMaxLength = 100;
  public static readonly EmailMaxLength = 50;
  public static readonly PhoneNumberMaxLength = 25;
  public static readonly userPasswordMinLength = 6;
  public static readonly userPasswordMaxLength = 30;
  public static readonly userNameMaxLength = 30;
  public static readonly PhoneNumberRegEx = /((?:\+|00)[17](?: |\-)?|(?:\+|00)[1-9]\d{0,2}(?: |\-)?|(?:\+|00)1\-\d{3}(?: |\-)?)?(0\d|\([0-9]{3}\)|[1-9]{0,3})(?:((?: |\-)[0-9]{2}){4}|((?:[0-9]{2}){4})|((?: |\-)[0-9]{3}(?: |\-)[0-9]{4})|([0-9]{7}))/;

  public static readonly PrjectName = 30;
  public static readonly ProductName = 14;
  public static readonly PresentationTeam = 1000;
  public static readonly ProjectAdress = 100;
  public static readonly FundingGoal = 12;
  public static readonly PercentageMaxLength = 3;
  public static readonly PercentageAsset = /^(100([\.][0]{1,})?$|[0-9]{1,2}([\.][0-9]{1,})?)$/;
  public static readonly ProjectDescreption = 300;
  public static readonly MaxCarouselProjectsNumber = 2;
  public static readonly maxFavoriteCount = 9;

  public static readonly coinValue = 1;

  public static readonly googleApiKey = "";

  public static readonly syncDataTime = 10000;
  public static readonly eur = "EUR";
}
