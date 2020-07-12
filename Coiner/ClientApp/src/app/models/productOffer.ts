import { OfferTypeEnum } from "./enums/offerType";

export class ProductOffer {
    offerAssetName: string;
    offerQuantity: number;
    askAssetName: string;
    askQuantity: number;
    pxCoin: number;
    fromAddress: string;
    txId: string;
    hexBlob: string;
    offerTpye?: OfferTypeEnum;
}