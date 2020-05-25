import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Chip from "@material-ui/core/Chip";

const useStyles = makeStyles((theme) => ({
    root: {
        display: 'flex',
        justifyContent: 'center',
        flexWrap: 'wrap',
        '& > *': {
            marginRight: theme.spacing(0.5),
        },
    },
}));

export const TopicTokens = ({ tokens, color }) => {

    const classes = useStyles();

    const tokenViews = tokens.map((token, idx) => (
        <Chip
            variant="outlined"
            style={{ color: color, border: `1px solid ${color}` }}
            key={idx}
            size="small"
            label={token} />
    ));

    return (
        <span className={classes.root}>{tokenViews}</span>
    );
};

export default TopicTokens;