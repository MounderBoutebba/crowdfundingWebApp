import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'firstUpper',

})

export class firstUpperPipe implements PipeTransform {
  transform(value: string) {
    if (!value) return null;
    return value.charAt(0).toUpperCase() + value.slice(1);

  }
}
