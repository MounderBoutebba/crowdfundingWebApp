import { NouiFormatter } from "ng2-nouislider";

export class CustomTooltip implements NouiFormatter {
    to(value: number): string {
      let output = Math.ceil(value).toString();
      return output;
    }
  
    from(value: string): number {
      return Number(value.split(" ")[0]);
    }
  }