import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
baseUrl='http://localhost:5016/api/';
private currentUserSource=new BehaviorSubject<User|null>(null);
currentUser$=this.currentUserSource.asObservable();

  constructor(private http:HttpClient) { }







  Login(model:any)
  {
    return this.http.post<User>(this.baseUrl+'Account/Login',model).pipe(
      map((response:User)=>{
      const user=response;
      if(user)
      {
        localStorage.setItem('user',JSON.stringify(user));
        this.currentUserSource.next(user);
      }

      })
    )
  }




  Register(model:any)
  {
    return this.http.post<User>(this.baseUrl+'Account/Register',model).pipe(
      map((user)=>{
        
      if(user)
      {
        localStorage.setItem('user',JSON.stringify(user));
        this.currentUserSource.next(user);
      }
    

      })
    )
  }



  setCurrentUser(user:User)
  {
    this.currentUserSource.next(user);
  }
  
  Logout()
  {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
