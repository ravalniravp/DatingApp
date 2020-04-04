import { Routes, RouteConfigLoadEnd } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member-List/member-List.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';



export const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent },
            { path: 'messages', component: MessagesComponent },
            { path: 'lists', component: ListsComponent },
        ]
    },

    { path: '**', redirectTo: '', pathMatch: 'full' }
];