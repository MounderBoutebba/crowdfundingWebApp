import { Product } from '../models/Product';
import { Project } from "../models/project";

export const Product1Mock: Product = {
    productName: 'test prod',
    lastTransaction: 10,
    totalCapitalization: 500,
    transactions: [1, 2, 3],
    project: new Project(),
    transactionVariation: 12,
    coinValue: 5,
    totalCoinNumber: 15,
    minBuyValue: '14',
    maxSellValue: '40'
};
export const Product2Mock: Product = {
    productName: 'product 2',
    lastTransaction: 24,
    totalCapitalization: 410,
    transactions: [15, 12, 13],
    project: new Project(),
    transactionVariation: 19,
    coinValue: 10,
    totalCoinNumber: 11,
    minBuyValue: '15',
    maxSellValue: '88'
};
export const mockProducts: Product[] = [Product1Mock, Product2Mock];