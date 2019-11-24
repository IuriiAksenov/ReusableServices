namespace ReusableServices.Common.Response
{
  public static class Codes
  {
    public const string MailSent = "mail_sent";

    public const string OperatorAnalytics = "operator_analytics";
    public const string OperatorsAnalytics = "operators_analytics";

    public const string IncomeQuestionChangesSynced = "income_question_changes_synced";
    public const string OutcomeQuestionChangesSyncing = "outcome_question_changes_syncing";
    public const string OutcomeQuestionChangesSynced = "outcome_question_changes_synced";

    public const string IncomeCompanyChangesSynced = "income_company_changes_synced";

    public const string Article = "article";
    public const string ArticlePaths = "article_paths";
    public const string ArticleMetadata = "article_metadata";
    public const string AddArticle = "add_article";
    public const string DeleteArticle = "delete_article";
    public const string UpdateArticleStatus = "update_article_status";
    public const string UpdateArticle = "update_article";
    public const string MarkArticle = "mark_article";
    public const string Folder = "folder";
    public const string AddFolder = "add_folder";
    public const string DeleteFolder = "delete_folder";
    public const string UpdateFolderStatus = "update_folder_status";
    public const string UpdateFolder = "update_folder";
    public const string Item = "item";
    public const string Search = "search";
    public const string Intents = "intents";
    public const string AddFaqs = "add_faqs";
    public const string Scroll = "scroll";
    public const string SearchItem = "search_item";

    public const string SignIn = "sign_in";
    public const string ChangePassword = "change_password";
    public const string PasswordChangeRequired = "password_change_required";
    public const string PasswordChanged = "password_changed";

    public const string UpdateTransitionFromConnect = "update_transition_from_connect";

    public const string QuestionCreated = "question_created";
    public const string QuestionMarked = "question_marked";
    public const string QuestionNotMarked = "question_not_marked";
    public const string MessageAdded = "message_added";

    public const string ChangeRole = "change_role";
  }

  public static class ErrorCodes
  {
    public const string FaqsAlreadyExisted = "faqs_already_existed_400";
    public const string AccessDenied = "access_denied";
    public const string ArticleNotFound = "article_not_found";
    public const string ArticleIsDefault = "article_is_default";
    public const string ArticleIsBlocked = "article_is_blocked";
    public const string OpenedArticleNotFound = "opened_article_not_found";
    public const string FileNotFound = "file_not_found";
    public const string FolderNotFound = "folder_not_found";
    public const string FolderIsDefault = "folder_is_default";
    public const string FolderIsBlocked = "folder_is_blocked";
    public const string ForbiddenAddArticle = "forbidden_add_article";
    public const string ForbiddenModifyArticle = "forbidden_modify_article";
    public const string ForbiddenDeleteArticle = "forbidden_delete_article";
    public const string ForbiddenAddFolder = "forbidden_add_folder";
    public const string ForbiddenModifyFolder = "forbidden_modify_folder";
    public const string ForbiddenDeleteFolder = "forbidden_delete_folder";
    public const string InvalidParameters = "invalid_parameters";
    public const string FolderIdEqualsParentId = "folderid_equals_parentid";
    public const string SearchNotFound = "search_not_found";

    public const string ExpiredItsAgreement = "expired_its_agreement";
    public const string LowItsLevel = "low_its_level";
    public const string CurrentPasswordNotValid = "current_password_not_valid";
    public const string InvalidEmailAndConnectLogin = "invalid_email_and_connect_login";

    public const string LoginInUse = "login_in_use";
    public const string EmailInUse = "email_in_use";
    public const string TokenNotFound = "token_not_found";
    public const string AdminAlreadyExisted = "admin_already_existed";
    public const string BadLoginOrPassword = "bad_login_or_password";
    public const string InvalidUserCredentials = "invalid_user_credentials";
    public const string InvalidPartnerCredentials = "invalid_partner_credentials";
    public const string InvalidSecretKey = "invalid_secret_key";
    public const string InvalidCurrentPassword = "invalid_current_password";
    public const string InvalidEmail = "invalid_email";
    public const string InvalidPassword = "invalid_password";
    public const string InvalidRole = "invalid_role";
    public const string RefreshTokenNotFound = "refresh_token_not_found";
    public const string RefreshTokenAlreadyRevoked = "refresh_token_already_revoked";

    public const string ForbiddenNews = "forbidden_news";
    public const string NotFoundNews = "news_not_found";
    public const string NewsAlreadyMarked = "news_already_marked";

    public const string SupportLineNotFound = "support_line_not_found";
    public const string MessageNotFound = "message_not_found";
    public const string QuestionNotFound = "question_not_found";
    public const string QuestionForbidden = "question_forbidden";
    public const string QuestionAlreadyResolved = "question_already_resolved";
    public const string QuestionNotInResolved = "question_not_in_resolved";
    public const string QuestionAlreadyMarked = "question_already_marked";

    public const string UserNotFound = "user_not_found";
    public const string PartnerNotFound = "partner_not_found";
    public const string ClientNotFound = "client_not_found";
    public const string CompanyNotFound = "company_not_found";
    public const string AdminNotFound = "admin_not_found";
    public const string OperatorNotFound = "operator_not_found";
    public const string DefaultOperatorNotFound = "default_operator_not_found";
    public const string ProviderNotFound = "rovider_not_found";
    public const string AdminNotSpecified = "admin_not_specified";
    public const string AdminExists = "admin_exists";
    public const string ProviderNotApproved = "provider_not_approved";
    public const string CompanyNotApproved = "company_not_approved";
    public const string OperatorNotApproved = "operator_not_approved";
    public const string ClientNotApproved = "client_not_approved";
  }
}