import { User } from "./user";
import { Project } from "./project";

export class Bill {
    id: number;
    pdfPath: string;
    userId: number;
    user: User;
    project: Project;
    projectId: number;
    content: any;
    creationDate: Date;
}