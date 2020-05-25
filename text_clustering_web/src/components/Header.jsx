import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';

const useStyles = makeStyles((theme) => ({
    header: {
        backgroundColor: theme.palette.primary.main,
        height: 210,
        color: theme.palette.common.white,
        marginBottom: 16
    },
    spaced: {
        marginBottom: 16
    }
}));

export const Header = ({ headerText, children }) => {

    const classes = useStyles();

    return (
        <Grid
            container
            className={classes.header}
            justify="flex-end"
            direction="column"
            alignItems="center">
            <Typography
                className={classes.spaced}
                variant="h4">
                {headerText}
            </Typography>
            {children}
        </Grid>
    );
};

export default Header;