import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  if(accountService.currentUser()){
    return of(true); //needs to do it so it waits for the observable to resolve and no need to subscribe as we are in a guard and it handles the subscription internally
  }
  else{
    return accountService.getAuthState().pipe(
      map(auth => {
        if(auth.isAuthenticated){
          return true;
        }
        else{
          router.navigate(['/account/login'], {queryParams: {returnUrl: state.url}});
          return false;
        }
      })
    )
  }
};
