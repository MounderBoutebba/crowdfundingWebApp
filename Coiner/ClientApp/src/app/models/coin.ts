import { CoinStatusEnum } from "./enums/coinStatusEnum";

export class Coin {
    id: number;
    coinValue: number;
    coinStatus: CoinStatusEnum;
    coinsNumber: number;
    usedNumber: number;
    coinPrice: number;
    coinsMonetizedNumber: number;
    userId: number;
    projectId: number;
}