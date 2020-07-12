import { Coin } from './coin';
export class InvestWithCryptoEuroDto {
    coin: Coin;
    userId: number;
    cryptoEuroQuantity: number;
    privateKey: string;
    ownerProjectWalletId: string;
}