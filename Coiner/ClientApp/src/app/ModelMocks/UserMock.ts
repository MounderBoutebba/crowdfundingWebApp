import { User } from './../models/user'

let date = new Date('06/09/1993');
export const User1Mock: User = {
    id: 2,
    firstName: 'user1',
    lastName: 'user1',
    userType: 0,
    gender: 1,
    birthDay: date,
    email: 'exemple@email.com',
    login: '1',
    password: 'azerty',
    phoneNumber: '031568451',
    job: 'jobless',
    userImage: undefined,
    address: 'Adress1Mock',
    confirmPassword: 'azerty',
    userCoinsNumber: 350,
    blockChainAddress: '1za5z11az1za1za1az1d5',
    blockChainPublicKey: '1zed1aze1az1f5f2az3',
    siren: '',
    tva: '',
    provider: 0,
    kyc: false,
    KycNotificationSent: false,
    walletId: ''
}

export const User2Mock: User = {
    id: 3,
    firstName: 'user2',
    lastName: 'user2',
    userType: 0,
    gender: 1,
    birthDay: date,
    email: 'exemple2@email.com',
    login: '2',
    password: 'azerty',
    phoneNumber: '078756451',
    job: 'doctor',
    userImage: undefined,
    address: 'Adress2Mock',
    confirmPassword: 'azerty',
    userCoinsNumber: 100,
    blockChainAddress: 'arfs2272yhrthza1az1d5',
    blockChainPublicKey: 'ferg87782reazedqsdazd',
    siren: '',
    tva: '',
    provider: 0,
    kyc: false,
    KycNotificationSent: false,
    walletId: ''
}
