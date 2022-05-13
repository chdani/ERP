/* tslint:disable:max-line-length */
import { FuseNavigationItem } from '@fuse/components/navigation';

export const defaultNavigation: FuseNavigationItem[] = [
    {
        id   : 'dashboard',
        title: 'Dashboard',
        type : 'basic',
        link : '/dashboard'
    },
    {
        id      : 'Finance',
        title   : 'Finance',
        type    : 'collapsable',
        children: [
            {
                id   : 'Budget',
                title: 'Budget',
                type : 'basic',
                link : '/dashboard/finance/budget'
            }
        ]
    },
    {
        id      : 'user-management',
        title   : 'User Management',
        type    : 'collapsable',
        children: [
            {
                id   : 'app-access',
                title: 'App Access',
                type : 'basic',
                link : '/user-management/app-access'
            },
            {
                id   : 'user-role',
                title: 'User Role',
                type : 'basic',
                link : '/user-management/user-role'
            },
            {
                id   : 'app-access-role-map',
                title: 'App Access Role',
                type : 'basic',
                link : '/user-management/app-access-role-map'
            },
            {
                id   : 'user',
                title: 'User',
                type : 'basic',
                link : '/user-management/user'
            },
            {
                id   : 'use-role',
                title: 'User Role Map',
                type : 'basic',
                link : '/user-management/use-role'
            },
            
            {
                id   : 'user-permissions',
                title: 'User Permissions',
                type : 'basic',
                link : '/user-management/user-permissions'
            },
               {
                id   : 'app-master-menu',
                title: 'App Master Menu',
                type : 'basic',
                link : '/user-management/app-menu-master'
            }

        ]
    },
   
];
export const compactNavigation: FuseNavigationItem[] = [
    {
        id   : 'example',
        title: 'Example',
        type : 'basic',
        icon : 'heroicons_outline:chart-pie',
        link : '/example'
    }
];
export const futuristicNavigation: FuseNavigationItem[] = [
    {
        id   : 'example',
        title: 'Example',
        type : 'basic',
        icon : 'heroicons_outline:chart-pie',
        link : '/example'
    }
];
export const horizontalNavigation: FuseNavigationItem[] = [
    {
        id   : 'example',
        title: 'Example',
        type : 'basic',
        icon : 'heroicons_outline:chart-pie',
        link : '/example'
    }
];
