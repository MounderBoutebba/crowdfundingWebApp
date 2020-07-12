import { User } from "./user";

export class Discussion {
    id: number;
    questionContent:string;
    answerContent:string;
    questionCreation:Date;
    answerCreation:Date;
    userId: number;
    projectId: number;
    creationDate: Date;
    user: User;
}