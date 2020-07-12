import { ProductOffer } from "./productOffer";

export class OfferDto {
    userId: number;
    privateKey: string;
    productOffer: ProductOffer;
    productName: string;
    commissionFees: number;
}