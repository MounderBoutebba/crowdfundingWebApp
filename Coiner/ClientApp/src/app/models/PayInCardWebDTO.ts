export class PayInCardWebDTO {
    cardType: string;
    cardId: string;
    templateURL: string;
    culture: CultureCode;
    secureMode: string;
    redirectURL: string;
    returnURL: string;
    statementDescriptor: string;
}

enum CultureCode {
    NotSpecified,
    DE,
    EN,
    DA,
    ES,
    ET,
    FI,
    FR,
    EL,
    HU,
    IT,
    NL,
    NO,
    PL,
    PT,
    SK,
    SV,
    CS
}