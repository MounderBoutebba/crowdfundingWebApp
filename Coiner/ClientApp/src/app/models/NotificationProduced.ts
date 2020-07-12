import { Project } from "./project";

export class NotificationProduced {
    Id: number;
    Title:string;
    Content: string;
    CreateDate: Date;
    ProjectId: number;
    UserId: number;
    ReadStatus:boolean;
}