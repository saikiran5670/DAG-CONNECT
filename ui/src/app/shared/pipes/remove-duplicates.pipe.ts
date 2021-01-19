import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'removeDuplicates'
})
export class RemoveDuplicatesPipe implements PipeTransform {

  transform(value: any, args?: any[]): any {
     // Remove the duplicate elements (this will remove duplicates
     let uniqueElements = value.filter(function (el, index, array) { 
      return array.indexOf (el) == index;
    });

  return uniqueElements;   
  }

}
