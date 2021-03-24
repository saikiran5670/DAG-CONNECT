import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthenticationComponent } from './authentication.component';
import { LoginComponent } from './login/login.component';
import { SetPasswordComponent } from './set-password/set-password.component';

const routes: Routes = [
  {
    path: "", component: AuthenticationComponent, children:[
        { path: "login", component: LoginComponent },
        { path: "createpassword/:token", component: SetPasswordComponent/*, children: [{path: "**", redirectTo: "/auth/createpassword" }]*/},
        { path: "resetpassword/:token", component: SetPasswordComponent/*, children: [{path: "**", redirectTo: "/auth/resetpassword" }]*/},
        { path: "resetpasswordinvalidate/:token", component: SetPasswordComponent/*, children: [{path: "**", redirectTo: "/auth/resetpassword" }]*/},
  ]
  }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthenticationRoutingModule { }
