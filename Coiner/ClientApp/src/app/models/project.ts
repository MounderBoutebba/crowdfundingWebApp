import { ProjectUpdate } from './projectupdate';
import { ProjectStatusEnum } from "./enums/projectStatusEnum";
import { ProjectTypeEnum } from "./enums/projectTypeEnum";
import { User } from "./user";
import { UploadedDocument } from "./uploadedDocument";
import { ProjectImage } from "./projectImage";
import { ActivityTypeEnum } from "./enums/activityTypeEnum";
import { Coin } from "./coin";
import { Slider } from "./slider";
import { Discussion } from './discussion';


export class Project {
    id: number;
    projectName: string;
    projectDescription: string;
    projectAddress: string;
    projectStatus: ProjectStatusEnum;
    fundraisingPeriod: number;
    projectType: ProjectTypeEnum;
    activityType: string;//ActivityTypeEnum;
    fundingGoal: number;
    receivedFunding: number;
    webLink: string;
    percentageAsset: number;
    // this is only for Project, Career, Product
    projectPresentation: string;
    // this is only for Project, Career, Society
    businessPlan: string;
    product_SalesPercepective: string;
    career_EngagementYears: number;
    society_Name: string;
    society_LegaleIdentification: string;
    society_StructureType: string;
    society_CreationDate: Date;
    beginEstimatedDate: Date;
    beginEnchereDate: Date;
    endEnchereDate: Date;
      // product name in blockchain
    productName: string;
    product_TVA: number;
    commissionTokenStirblock: number;
    product_BillDescription: string;
    userId: number;
    user: User;
    projectImages: ProjectImage[];
    documents: UploadedDocument[];
    projectUpdates: ProjectUpdate[];
    coins: Coin[];
    discussions: Discussion[];
    slider: Slider;
    backers: number;
}
