import React from 'react';
import CardHeader from '@material-ui/core/CardHeader';
import IconButton from '@material-ui/core/IconButton';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import WbIncandescentTwoToneIcon from '@material-ui/icons/WbIncandescentTwoTone';

export const DatasetCardHeader = ({ data }) => {

    return (
        <CardHeader
            avatar={
                <WbIncandescentTwoToneIcon color="primary" fontSize="large" />
            }
            action={
                <IconButton aria-label="settings">
                    <MoreVertIcon />
                </IconButton>
            }
            title={`${data.datasetName} (${data.textCount}/${data.clusterCount})`}
            subheader="September 14, 2016"
        />
    );
};

export default DatasetCardHeader;