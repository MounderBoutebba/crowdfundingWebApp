import { User } from "./user";
import { RedirectPageEnum } from "./enums/redirectPageEnum";

export class CreditCurrencyDto {
    userId: number;
    currencyQuantity: number;
    walletId: string;
    projectId: number;
    redirectPage: RedirectPageEnum;
}