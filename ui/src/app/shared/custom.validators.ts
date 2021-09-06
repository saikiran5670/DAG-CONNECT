import { AbstractControl, FormControl, FormGroup } from '@angular/forms';
import { objectWithoutKey } from 'angular-slickgrid';

export class CustomValidators {
  static emailDomain(domainName: string) {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const email: string = control.value;
      const domain = email.substring(email.lastIndexOf('@') + 1);
      if (email === '' || domain.toLowerCase() === domainName.toLowerCase()) {
        return null;
      } else {
        return { emailDomain: true };
      }
    };
  }

  // static validatePassword(newPassword) {
  //   return (formGroup: FormGroup) => {
  //     const NEW_PASSWORD = formGroup.controls[newPassword];
  //   var upper = /[A-Z]/,
  //       lower = /[a-z]/,
  //       number = /[0-9]/,
  //       special = /[ !"#$%&'()*+,\-./:;<=>?@[\\\]^_`{|}~]/;

  //   if (!upper.test(NEW_PASSWORD.value) ||
  //       !lower.test(NEW_PASSWORD.value) ||
  //       !number.test(NEW_PASSWORD.value) ||
  //       !special.test(NEW_PASSWORD.value)
  //   ) {
  //     NEW_PASSWORD.setErrors({ passwordInvalid: true });;
  //   }

  //   return;
  //   }
  // }

  static validatePassword(newPassword) {
    return (formGroup: FormGroup) => {
      const NEW_PASSWORD = formGroup.controls[newPassword];
      var upper = /[A-Z]/,
        lower = /[a-z]/,
        number = /[0-9]/,
        special = /[ !"#$%&'()*+,\-/:;<=>?@[\\\]^_`{|}~]/;

      // let specialCharRequired: boolean = false;
      // let numberRequired: boolean = false;
      // let lowercaseRequired: boolean = false;
      // let uppercaseRequired: boolean = false;
      let customError = {};
      if (!special.test(NEW_PASSWORD.value)) {//errors.push(special);
        // specialCharRequired = true;
        customError['specialCharRequired']=true;
      } else {
        customError['specialCharRequired'] ? delete customError['specialCharRequired'] : '';
      }
      if (!number.test(NEW_PASSWORD.value)) {
        // numberRequired = true;
        customError['numberRequired']=true;
      } else {
        customError['numberRequired'] ? delete customError['numberRequired'] : '';
      }
      if (!lower.test(NEW_PASSWORD.value)) {
        // lowercaseRequired = true;
        customError['lowercaseRequired']=true;
      } else {
        customError['lowercaseRequired'] ? delete customError['lowercaseRequired'] : '';
      }
      if (!upper.test(NEW_PASSWORD.value)) {
        // uppercaseRequired = true;
        customError['uppercaseRequired']=true;
      } else {
        customError['uppercaseRequired'] ? delete customError['uppercaseRequired'] : '';
      }
      // let customError = { specialCharRequired: specialCharRequired, numberRequired: numberRequired, lowercaseRequired: lowercaseRequired, uppercaseRequired: uppercaseRequired};
      let allErrors = {...NEW_PASSWORD.errors,...customError};
      
      if(Object.keys(allErrors).length==0) {
        NEW_PASSWORD.setErrors(null);
        return false;
      }
      NEW_PASSWORD.setErrors(allErrors);
      return true;
    };
  }

  static mustMatchNewAndConfirmPassword(
    newPassword: string,
    confirmPassword: string
  ) {
    return (formGroup: FormGroup) => {
      const NEW_PASSWORD = formGroup.controls[newPassword];
      const CONFIRM_PASSWORD = formGroup.controls[confirmPassword];

      if (CONFIRM_PASSWORD.errors && !CONFIRM_PASSWORD.errors.mustMatch) {
        // return if another validator has already found an error on the CONFIRM_PASSWORD
        return;
      }

      // set error on CONFIRM_PASSWORD if validation fails
      if (NEW_PASSWORD.value !== CONFIRM_PASSWORD.value) {
        CONFIRM_PASSWORD.setErrors({ mustMatch: true });
      } else {
        CONFIRM_PASSWORD.setErrors(null);
      }
    };
  }

  // static checkForCurrentPassword(
  //   currentPassword: string,
  //   password: string,
  //   userService: EmployeeService
  // ) {
  //   password;
  //   userService.changePassword().subscribe((data) => {
  //     password = data[0].currentPassword;
  //   });
  //   return (formGroup: FormGroup) => {
  //     const CURRENT_PASSWORD = formGroup.controls[currentPassword];

  //     if (CURRENT_PASSWORD.errors && !CURRENT_PASSWORD.errors.wrongPassword) {
  //       return;
  //     }

  //     if (CURRENT_PASSWORD.value != password) {
  //       CURRENT_PASSWORD.setErrors({ wrongPassword: true });
  //     } else {
  //       CURRENT_PASSWORD.setErrors(null);
  //     }
  //     return;
  //   };
  // }

  static specialCharValidationForName(name, formArrayName?) {
    
    return (formGroup: FormGroup) => {
      let NAME;
      if(formArrayName){
        NAME = formGroup.controls[formArrayName]['controls'][0]['controls'][name];
      }else{
        NAME = formGroup.controls[name];
      }
      var regex = /[!@#\$%&*]/;

      if (!NAME.value) {
        NAME.setErrors({ required: true });
      } else if (regex.test(NAME.value)) {
        NAME.setErrors({ specialCharsNotAllowed: true });
      }
    };
  }

  static specialCharValidationForNameWithoutRequired(name: any) {
    return (formGroup: FormGroup) => {
      const NAME = formGroup.controls[name];
      var regex = /[!@#\$%&*]/;

      if(NAME.value.length > 0){
        if (regex.test(NAME.value)) {
          NAME.setErrors({ specialCharsNotAllowed: true });
        }
      }
    };
  }

  static specialCharValidationForMail(name) {
    
    return (formGroup: FormGroup) => {
      let NAME;
      
        NAME = formGroup.controls[name];
      var regex = /[`~!@#\$%&*:;"']/;

      if (!NAME.value) {
        NAME.setErrors({ required: true });
      } else if (regex.test(NAME.value)) {
        NAME.setErrors({ specialCharsNotAllowed: true });
      }
    };
  }

  static numberValidationForName(name) {
    return (formGroup: FormGroup) => {
      const NAME = formGroup.controls[name];
      var regex = /[0-9]/;

      if (!NAME.value) {
        NAME.setErrors({ required: true });
      } else if (regex.test(NAME.value)) {
        NAME.setErrors({ numberNotAllowed: true });
      }
    };
  }

  static numberValidationForNameWithoutRequired(name: any) {
    return (formGroup: FormGroup) => {
      const NAME = formGroup.controls[name];
      var regex = /[0-9]/;
      if(NAME.value.length > 0){
        if (regex.test(NAME.value)) {
          NAME.setErrors({ numberNotAllowed: true });
        }
      }
    };
  }

  static noWhitespaceValidator(control: FormControl) {
    const isWhitespace =
      ((control && control.value && control.value.toString()) || '').trim()
        .length === 0;
    const isValid = !isWhitespace;
    return isValid ? null : { whitespace: true };
  }

  static noWhitespaceValidatorWithoutRequired(control: FormControl) {
    let isWhitespace =
      ((control && control.value && control.value.toString()) || '').trim()
        .length === 0;
    if(control.value.length == 0){
      isWhitespace = false;
    }
    const isValid = !isWhitespace;
    return isValid ? null : { whitespace: true };
  }

  static noWhitespaceValidatorforDesc(control: FormControl) {
    const isSpace = (control.value || '').match(/^(\s+\S+\s*)*(?!\s).*$/);
    return isSpace ? null : { whitespace: true };
  }

  static validateImageFile(inputFile): string{
    let imageError= '';
    const max_size = 1024*200;
    if (inputFile.size > max_size) {
      imageError = 'Maximum size allowed is ' + max_size / (1024) + 'Kb';

    }

    if (!(inputFile.type).includes("image")) {
        imageError = 'Only Images are allowed';
    }
    return imageError;
  }

  static numberFieldValidation(name,maxValue){
    return (formGroup: FormGroup) => {
      const NAME = formGroup.controls[name];
      var regex = /[0-9]/;

      if (NAME.value) {
       if(NAME.value > maxValue){
          NAME.setErrors({ cannotExceedMaxValue: true });
        }
        else if(NAME.value < 0){
          NAME.setErrors({ noNegativeValueAllowed: true });

        }
      }
    };
  }

  static numberMinFieldValidation(name,minValue){
    return (formGroup: FormGroup) => {
      const NAME = formGroup.controls[name];
      var regex = /[0-9]/;

      if (NAME.value) {
       if(NAME.value < minValue){
          NAME.setErrors({ cannotExceedMinValue: true });
        }
      }
    };
  }

  static timeMaxFieldValidation(name,maxValue){
    // let time = this.backToSeconds(maxValue);
    return (formGroup: FormGroup) => {
      const NAME = formGroup.controls[name];
      var regex = /[0-9]/;

      if (NAME.value) {
       if(this.backToSeconds(NAME.value) > maxValue){
          NAME.setErrors({ timeCannotExceedMaxValue: true });
        }
        else if(this.backToSeconds(NAME.value) < 0){
          NAME.setErrors({ noNegativeTimeValueAllowed: true });

        }
      }
    };
  }

  static timeMinFieldValidation(name,minValue){
    // let time = this.backToSeconds(minValue);
    return (formGroup: FormGroup) => {
      const NAME = formGroup.controls[name];
      var regex = /[0-9]/;

      if (NAME.value) {
       if(this.backToSeconds(NAME.value) < minValue){
          NAME.setErrors({ timeCannotExceedMinValue: true });
        }
      }
    };
  }

  static backToSeconds(time: any){
    const hms = time;
    const [hours, minutes, seconds] = hms.split(':');
    const totalSeconds = (+hours) * 60 * 60 + (+minutes) * 60 + (+seconds);
    return totalSeconds;
  }
}
