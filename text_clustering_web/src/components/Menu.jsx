import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Drawer from '@material-ui/core/Drawer';
import List from '@material-ui/core/List';
import Divider from '@material-ui/core/Divider';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import ExitToApp from '@material-ui/icons/ExitToApp';
import { Link } from "react-router-dom";
import GroupIcon from '@material-ui/icons/Group';
import DashboardIcon from '@material-ui/icons/Dashboard';
import PublishIcon from '@material-ui/icons/Publish';

const useStyles = ({ drawerWidth }) => makeStyles((theme) => ({
    root: {
        display: 'flex'
    },
    drawer: {
        width: drawerWidth,
        flexShrink: 0
    },
    drawerPaper: {
        width: drawerWidth
    },
    drawerHeader: theme.mixins.toolbar,
    a: {
        textDecoration: "none",
        color: "inherit"
    }
}));

export const Menu = ({ open, drawerWidth, onLogout }) => {

    const classes = useStyles({ drawerWidth })();

    const routes = [
        {
            text: 'Головна',
            to: '/dashboard',
            icon: (<DashboardIcon />)
        },
        {
            text: 'Завантажити',
            to: '/upload',
            icon: (<PublishIcon />)
        },
        {
            text: 'Адміністрування',
            to: '/upload',
            icon: (<GroupIcon />)
        }
    ];

    return (
        <div className={classes.root}>
            <Drawer
                className={classes.drawer}
                variant="persistent"
                anchor="left"
                open={open}
                classes={{
                    paper: classes.drawerPaper,
                }}
            >
                <div className={classes.drawerHeader} />
                <Divider />
                <List>
                    {routes.map(({ text, to, icon }, index) => (
                        <Link
                            to={to}
                            key={index}
                            className={classes.a}
                        >
                            <ListItem button>
                                <ListItemIcon>{icon}</ListItemIcon>
                                <ListItemText primary={text} />
                            </ListItem>
                        </Link>
                    ))}
                </List>
                <Divider />
                <List>
                    <ListItem button onClick={onLogout}>
                        <ListItemIcon><ExitToApp /></ListItemIcon>
                        <ListItemText primary={"Вихід"} />
                    </ListItem>
                </List>
            </Drawer>
        </div>
    );
}