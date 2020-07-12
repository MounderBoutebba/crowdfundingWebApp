import { ActivityTypeEnum } from "./enums/activityTypeEnum";
import { ProjectTypeEnum } from "./enums/projectTypeEnum";

export class ProjectFilterDto {
    projectTypes: ProjectTypeEnum[] = [];
    projectActivityTypes: string[] = [];
    pageIndex: number;
    pageSize: number;
}