import { OfferTypeEnum } from "./enums/offerType";

export class BlockChainOfferDto {
    userId: number;
    privateKey: string;
    productName: string;
    currency: string;
    unitPrice: number;
    currencyQuantity: number;
    productQuantity: number;
    offerType: OfferTypeEnum;
    totalPrice?: number;
    commissionFees: number = 0;
}
