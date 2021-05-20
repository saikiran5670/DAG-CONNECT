import { AbstractControl, FormControl, FormGroup } from '@angular/forms';

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

      if (!special.test(NEW_PASSWORD.value)) {
        NEW_PASSWORD.setErrors({ specialCharRequired: true });
      }
      if (!number.test(NEW_PASSWORD.value)) {
        NEW_PASSWORD.setErrors({ numberRequired: true });
      }
      if (!lower.test(NEW_PASSWORD.value)) {
        NEW_PASSWORD.setErrors({ lowercaseRequired: true });
      }
      if (!upper.test(NEW_PASSWORD.value)) {
        NEW_PASSWORD.setErrors({ uppercaseRequired: true });
      }
      if (!NEW_PASSWORD.value) {
        NEW_PASSWORD.setErrors({ required: true });
      }
      return;
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
}
