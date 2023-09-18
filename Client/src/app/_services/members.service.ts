import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { UserParams } from '../_models/user-params';
import { User } from '../_models/user';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';
@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl=environment.apiUrl;
  members:Member[]=[];
  memberCache = new Map();
  userparams:UserParams|undefined;
  user:User | undefined;

  constructor(private http:HttpClient,private accountService:AccountService ) { 
 this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user=>
      {
        if(user)
        {
          this.userparams=new UserParams(user);
          this.user=user;
        }      
      }
    })

  }

/////this method services to cach the results
  // getMembers()
  // {
  //   if (this.members.length>0) return of(this.members)
  //   return this.http.get<Member[]>(this.baseUrl+'Users').pipe(
  //     map(
  //       members=>{
  //         this.members=members
  //         return this.members
  //       }
  //       )
  //   ) 
  // }



  getUserParams()
  {
    return this.userparams;
  }


  setUserParams(userparams:UserParams)
  {
    this.userparams=userparams;
  }


  resetUserParams()
  {
    if(this.user)
    {
      return this.userparams=new UserParams(this.user);
    }

    return;
  }



  getMembers(userParams:UserParams)
  {
    
    const response = this.memberCache.get(Object.values(userParams).join('-'));
    if (response) return of(response);
    let params = getPaginationHeader(userParams.pageNumber,userParams.pageSize);
             params = params.append('minAge', userParams.minAge);
             params = params.append('maxAge', userParams.maxAge);
             params = params.append('gender', userParams.gender);
             params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'Users',params,this.http).pipe(
     
      map(response => {  
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    )
    


  }





  getMember(username:string)
  {
    const member = [...this.memberCache.values()]
    .reduce((arr, elem) => arr.concat(elem.result), [])
    .find((member: Member) => member.userName === username);
     
      
    //const member=this.members.find(x=>x.userName===username);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl+'Users/'+username);  
  }

  updateMember(member: Member)
  {
    return this.http.put(this.baseUrl+'Users',member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = { ...this.members[index], ...member }
      })
    ) 
  }

  setMainPhoto(photoId: number)
  {
    return this.http.put(this.baseUrl + 'users/SetMainPhoto/' + photoId, {});
  }


  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/DeletePhoto/' + photoId);
  }



  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {})
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {

    let params = getPaginationHeader(pageNumber, pageSize);
    params = params.append('predicate', predicate);

    return getPaginatedResult<Member[]>(this.baseUrl + 'likes', params,this.http);
  }
  



}
