
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';


@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  activeTab?: TabDirective;
  member: Member ={} as Member;
  galleryOptions: NgxGalleryOptions[]=[];
  galleryImages: NgxGalleryImage[]=[];
  messages: Message[] = [];

  constructor(private memberService: MembersService,
     private route: ActivatedRoute,private messageService: MessageService) { }

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member']
    })

    this.route.queryParams.subscribe({
      next:params=>{
        params['tab'] && this.selectTab(params['tab'])
      }

    })


    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      },
    ]


    this.galleryImages=  this.getImages();
   
    
  }



  loadMessages() {
    if(this.member)
    {
      this.messageService.GetMessageThread(this.member.userName).subscribe({
        next: response => {
          this.messages = response;
       
        }
      })
    }
    
  }



  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages') {
      this.loadMessages();
    } 
  }








selectTab(heading: string) {
  if(this.memberTabs)
  {
    this.memberTabs.tabs.find(x=>x.heading==heading)!.active=true;
  }
}



getImages(){

  if (!this.member) return [];
  const imagesUrls=[];
    for (const photo of this.member?.photos) {
    imagesUrls.push({
      small: photo.url,
      medium: photo.url,
      big: photo.url
    });
    }

    return imagesUrls;
}



}
