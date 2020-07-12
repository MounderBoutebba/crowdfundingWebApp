import { ProviderEnum } from './enums/providerEnum';
import { Address } from './address';
import { GenderEnum } from "./enums/genderEnum";
import { UserImage } from "./userImage";
import { UserTypeEnum } from './enums/userType';

export class User {
    id: number;
    userType: UserTypeEnum;// ------------ new field for Pro users
    firstName: string;
    lastName: string;
    gender: GenderEnum;
    birthDay: Date;
    email: string;
    login: string;
    password: string;
    phoneNumber: string;
    job: string;
    userImage: UserImage;
    address: string;// ------------  new field for Pro users
    confirmPassword: string;
    userCoinsNumber: number;
    blockChainAddress: string;
    blockChainPublicKey: string;
    provider: ProviderEnum;
    siren: String;// ------------  new field for Pro users
    tva: String;// ------------  new field for Pro users
    kyc: boolean;
    KycNotificationSent: boolean;
    walletId: string;
}
