import { Project } from "./project";

export class Product {
    productName: string;
    lastTransaction: number;
    totalCapitalization: number;
    transactions: number[];
    project: Project;
    transactionVariation: number;
    coinValue:number;
    totalCoinNumber: number;
    minBuyValue:string;
    maxSellValue:string;
}